using UnityEngine;
using System.IO;

namespace GemmaCpp
{
    public enum GemmaWeightFormat
    {
        f32,    // 32-bit floating point
        bf16,   // Brain floating point (16-bit)
        sfp     // Structured floating point
    }

    [CreateAssetMenu(fileName = "GemmaSettings", menuName = "Gemma/Settings")]
    public class GemmaManagerSettings : ScriptableObject
    {
        [Header("Model Configuration")]
        [SerializeField] private GemmaModelType modelType = GemmaModelType.Gemma2B;
        [SerializeField] private GemmaWeightFormat weightFormat = GemmaWeightFormat.sfp;

        [Header("Model Files")]
        [SerializeField, Tooltip("Folder name in StreamingAssets containing the model files")]
        private string modelFolder = "gemma-2.0-2b";

        [SerializeField, Tooltip("Name of the tokenizer file (e.g., tokenizer.model)")]
        private string tokenizerFileName = "tokenizer.spm";

        [SerializeField, Tooltip("Name of the weights file")]
        private string weightsFileName = "2.0-2b-it-sfp.sbs";
        [SerializeField, Tooltip("Model type string")]
        private string modelFlag = "gemma2-2b-it";

        [Header("Generation Settings")]
        [SerializeField, Tooltip("Maximum number of tokens to generate")]
        private int maxTokens = 256;

        [SerializeField, Range(0f, 1f), Tooltip("Temperature for text generation (higher = more random)")]
        private float temperature = 0.7f;

        [SerializeField, Range(0f, 1f), Tooltip("Top-p sampling (nucleus sampling) threshold")]
        private float topP = 0.9f;

        // Properties
        public string ModelFlag => modelFlag;
        public GemmaWeightFormat WeightFormat => weightFormat;
        public string ModelPath => Path.Combine(Application.streamingAssetsPath, modelFolder);
        public string TokenizerPath => Path.Combine(ModelPath, tokenizerFileName);
        public string WeightsPath => Path.Combine(ModelPath, weightsFileName);
        public int MaxTokens => maxTokens;
        public float Temperature => temperature;
        public float TopP => topP;

        private void OnValidate()
        {
            // Validate paths
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(ModelPath))
            {
                if (!Directory.Exists(ModelPath))
                {
                    Debug.LogWarning($"Model folder not found: {ModelPath}");
                }
                else
                {
                    if (!File.Exists(TokenizerPath))
                    {
                        Debug.LogWarning($"Tokenizer file not found: {TokenizerPath}");
                    }
                    if (!File.Exists(WeightsPath))
                    {
                        Debug.LogWarning($"Weights file not found: {WeightsPath}");
                    }
                }
            }
#endif

            maxTokens = Mathf.Max(1, maxTokens);
            temperature = Mathf.Clamp(temperature, 0f, 1f);
            topP = Mathf.Clamp(topP, 0f, 1f);
        }
    }
}