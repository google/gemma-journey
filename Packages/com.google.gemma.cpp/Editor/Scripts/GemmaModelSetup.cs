using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

namespace GemmaCpp.Editor
{
    public static class GemmaModelSetup
    {
        [MenuItem("Gemma/Validate Model Setup")]
        public static void ValidateModelSetup()
        {
            // Ensure StreamingAssets exists
            var streamingAssetsPath = Application.streamingAssetsPath;
            if (!Directory.Exists(streamingAssetsPath))
            {
                Directory.CreateDirectory(streamingAssetsPath);
                Debug.Log("Created StreamingAssets directory");
            }

            // Check for model folders
            var modelFolders = Directory.GetDirectories(streamingAssetsPath)
                .Where(d => Path.GetFileName(d).StartsWith("gemma-"));

            if (!modelFolders.Any())
            {
                Debug.LogWarning(
                    "No Gemma model folders found in StreamingAssets.\n" +
                    "Please download a Gemma model and place it in:\n" +
                    $"{streamingAssetsPath}/gemma-2b\n" +
                    "or\n" +
                    $"{streamingAssetsPath}/gemma-7b"
                );
                return;
            }

            foreach (var modelPath in modelFolders)
            {
                ValidateModelFolder(modelPath);
            }
        }

        // @fixme unfortunately models on model hubs don't always match the files below so... what to do?
        private static void ValidateModelFolder(string modelPath)
        {
            var modelName = Path.GetFileName(modelPath);
            Debug.Log($"Checking model: {modelName}");

            // Note: We no longer enforce specific filenames
            // Instead, we check that at least one tokenizer and weight file exists
            bool hasTokenizer = Directory.GetFiles(modelPath, "*.spm").Length > 0;
            bool hasWeights = Directory.GetFiles(modelPath, "*.sbs").Length > 0;

            if (!hasTokenizer)
            {
                Debug.LogError($"No tokenizer file found in {modelName}");
            }
            if (!hasWeights)
            {
                Debug.LogError($"No weights file found in {modelName}");
            }
        }
    }
}