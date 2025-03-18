using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

namespace GemmaCpp
{
    public class GemmaManager : MonoBehaviour
    {
        [SerializeField]
        public GemmaManagerSettings settings;

        [SerializeField]
        private bool verboseLogging = false;

        private Gemma gemma;
        private bool isInitialized;

        private static int MAX_TOKENS = 131072;

        private void Start()
        {
            Debug.Log("GemmaManager: Starting initialization");
            InitializeGemma();
        }

        private void OnDestroy()
        {
            if (gemma != null)
            {
                gemma.Dispose();
                gemma = null;
                isInitialized = false;
                Debug.Log("GemmaManager: Resources cleaned up");
            }
        }

        private void InitializeGemma()
        {
            try
            {
                Debug.Log($"GemmaManager: Initializing with tokenizer: {settings.TokenizerPath}, weights: {settings.WeightsPath}");
                Debug.Log($"GemmaManager: Using model flag: {settings.ModelFlag}, weight format: {settings.WeightFormat}");

                gemma = new Gemma(
                    settings.TokenizerPath,
                    settings.ModelFlag,
                    settings.WeightsPath,
                    settings.WeightFormat.ToString(),
                    MAX_TOKENS // change for gemma3-1b
                );
                isInitialized = true;
                Debug.Log("GemmaManager: Initialized successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"GemmaManager: Error initializing - {e.Message}\n{e.StackTrace}");
            }
        }

        public async UniTask<string> GenerateResponseAsync(string prompt, Gemma.TokenCallback onTokenReceived = null)
        {
            if (!isInitialized)
            {
                Debug.LogError("GemmaManager: Cannot generate response - not initialized");
                throw new InvalidOperationException("Gemma is not initialized");
            }

            if (verboseLogging)
            {
                Debug.Log($"GemmaManager: Generating response for prompt: \"{TruncateForLogging(prompt)}\"");
            }

            // Create a callback that uses UniTask's PlayerLoopTiming to run on main thread
            Gemma.TokenCallback wrappedCallback = null;
            if (onTokenReceived != null)
            {
                wrappedCallback = (token) =>
                {
                    bool result = false;
                    UniTask.Post(() =>
                    {
                        result = onTokenReceived(token);
                        if (verboseLogging)
                        {
                            Debug.Log($"GemmaManager: Token received: \"{TruncateForLogging(token)}\"");
                        }
                    }, PlayerLoopTiming.Update);
                    return true;
                };
            }

            // Run the generation on a background thread
            Debug.Log("GemmaManager: Starting text generation on background thread");
            try
            {
                string result = await UniTask.RunOnThreadPool(() =>
                {
                    try
                    {
                        return gemma.Generate(prompt, wrappedCallback, MAX_TOKENS);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"GemmaManager: Generation failed - {e.Message}\n{e.StackTrace}");
                        throw new Exception($"Failed to generate response: {e.Message}", e);
                    }
                });

                if (verboseLogging)
                {
                    Debug.Log($"GemmaManager: Generation complete, result length: {result.Length} chars");
                }
                else
                {
                    Debug.Log("GemmaManager: Text generation complete");
                }

                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"GemmaManager: Exception during generation - {e.Message}\n{e.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Generate a response from Gemma using both text and image input
        /// </summary>
        /// <param name="prompt">The text prompt to send to Gemma</param>
        /// <param name="renderTexture">The RenderTexture containing the image to process</param>
        /// <param name="onTokenReceived">Optional callback to receive tokens as they're generated</param>
        /// <returns>The generated response</returns>
        public async UniTask<string> GenerateMultimodalResponseAsync(string prompt, RenderTexture renderTexture, Gemma.TokenCallback onTokenReceived = null)
        {
            if (!isInitialized)
            {
                Debug.LogError("GemmaManager: Cannot generate multimodal response - not initialized");
                throw new InvalidOperationException("Gemma is not initialized");
            }

            if (renderTexture == null)
            {
                Debug.LogError("GemmaManager: RenderTexture is null");
                throw new ArgumentNullException(nameof(renderTexture), "RenderTexture cannot be null");
            }

            Debug.Log($"GemmaManager: Starting multimodal generation with image {renderTexture.width}x{renderTexture.height}");
            if (verboseLogging)
            {
                Debug.Log($"GemmaManager: Multimodal prompt: \"{TruncateForLogging(prompt)}\"");
            }

            // Create a callback that uses UniTask's PlayerLoopTiming to run on main thread
            Gemma.TokenCallback wrappedCallback = null;
            if (onTokenReceived != null)
            {
                wrappedCallback = (token) =>
                {
                    bool result = false;
                    UniTask.Post(() =>
                    {
                        result = onTokenReceived(token);
                        if (verboseLogging)
                        {
                            Debug.Log($"GemmaManager: Multimodal token received: \"{TruncateForLogging(token)}\"");
                        }
                    }, PlayerLoopTiming.Update);
                    return true;
                };
            }

            try
            {
                // Convert RenderTexture to float array on the main thread
                Debug.Log("GemmaManager: Converting RenderTexture to float array");
                float[] imageData = await ConvertRenderTextureToFloatArrayAsync(renderTexture);
                Debug.Log($"GemmaManager: Converted image to float array, size: {imageData.Length} elements");

                // Run the generation on a background thread
                Debug.Log("GemmaManager: Starting multimodal generation on background thread");
                string result = await UniTask.RunOnThreadPool(() =>
                {
                    try
                    {
                        return gemma.GenerateMultimodal(prompt, imageData, renderTexture.width, renderTexture.height, wrappedCallback, MAX_TOKENS);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"GemmaManager: Multimodal generation failed - {e.Message}\n{e.StackTrace}");
                        throw new Exception($"Failed to generate multimodal response: {e.Message}", e);
                    }
                });

                if (verboseLogging)
                {
                    Debug.Log($"GemmaManager: Multimodal generation complete, result length: {result.Length} chars");
                }
                else
                {
                    Debug.Log("GemmaManager: Multimodal generation complete");
                }

                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"GemmaManager: Exception during multimodal generation - {e.Message}\n{e.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Converts a RenderTexture to a float array of RGB values in the range [0,1]
        /// </summary>
        /// <param name="renderTexture">The RenderTexture to convert</param>
        /// <returns>Float array of RGB values</returns>
        private async UniTask<float[]> ConvertRenderTextureToFloatArrayAsync(RenderTexture renderTexture)
        {
            // This needs to run on the main thread because it accesses Unity objects
            await UniTask.SwitchToMainThread();
            Debug.Log("GemmaManager: Starting RenderTexture conversion on main thread");

            int width = renderTexture.width;
            int height = renderTexture.height;
            Debug.Log($"GemmaManager: Processing image of size {width}x{height}");

            // Create a temporary texture to read the pixels
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            Debug.Log("GemmaManager: Created temporary Texture2D");

            // Remember the current active render texture
            RenderTexture previousActive = RenderTexture.active;

            try
            {
                // Set the render texture as active and read the pixels
                RenderTexture.active = renderTexture;
                texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                texture.Apply();
                Debug.Log("GemmaManager: Read pixels from RenderTexture");

                // Get the raw pixel data
                Color32[] pixels = texture.GetPixels32();
                Debug.Log($"GemmaManager: Got {pixels.Length} pixels from texture");

                // Convert to float array in the format expected by Gemma (RGB values in [0,1])
                float[] imageData = new float[pixels.Length * 3];
                for (int i = 0; i < pixels.Length; i++)
                {
                    imageData[i * 3 + 0] = pixels[i].r / 255.0f; // R
                    imageData[i * 3 + 1] = pixels[i].g / 255.0f; // G
                    imageData[i * 3 + 2] = pixels[i].b / 255.0f; // B
                }

                if (verboseLogging)
                {
                    // Log some sample pixel values to verify conversion
                    Debug.Log($"GemmaManager: Sample pixel values (first 3 pixels):");
                    for (int i = 0; i < Math.Min(3, pixels.Length); i++)
                    {
                        Debug.Log($"  Pixel {i}: R={imageData[i * 3 + 0]:F2}, G={imageData[i * 3 + 1]:F2}, B={imageData[i * 3 + 2]:F2}");
                    }
                }

                Debug.Log($"GemmaManager: Converted to float array with {imageData.Length} elements");
                return imageData;
            }
            catch (Exception e)
            {
                Debug.LogError($"GemmaManager: Error converting RenderTexture - {e.Message}\n{e.StackTrace}");
                throw;
            }
            finally
            {
                // Restore the previous active render texture
                RenderTexture.active = previousActive;
                Debug.Log("GemmaManager: Restored previous RenderTexture.active");

                // Clean up the temporary texture
                Destroy(texture);
                Debug.Log("GemmaManager: Destroyed temporary texture");
            }
        }

        /// <summary>
        /// Truncates a string for logging purposes to avoid flooding the console
        /// </summary>
        private string TruncateForLogging(string text, int maxLength = 50)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength) + "...";
        }
    }
}