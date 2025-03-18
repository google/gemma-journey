using System.Collections.Generic;

namespace GemmaCpp
{
    public static class GemmaModelUtils
    {
        // Model flag strings that map to model types
        private static readonly string[] ModelFlags = new[]
        {
            "2b-pt", "2b-it",                // Gemma 2B
            "7b-pt", "7b-it",                // Gemma 7B
            "gr2b-pt", "gr2b-it",            // RecurrentGemma
            "tiny",                          // Gemma Tiny
            "gemma2-2b-pt", "gemma2-2b-it",  // Gemma2 2B
            "9b-pt", "9b-it",                // Gemma2 9B
            "27b-pt", "27b-it",              // Gemma2 27B
            "gemma3-4b",                     // Gemma3 4B
            "gemma3-1b",                     // Gemma3 1B
            "gemma3-12b",                    // Gemma3 12B
            "gemma3-27b",                    // Gemma3 27B
            "paligemma-224",                 // PaliGemma 224
            "paligemma-448",                 // PaliGemma 448
            "paligemma2-3b-224",             // PaliGemma2 3B 224
            "paligemma2-3b-448",             // PaliGemma2 3B 448
            "paligemma2-10b-224",            // PaliGemma2 10B 224
            "paligemma2-10b-448",            // PaliGemma2 10B 448
        };

        // Corresponding model types for each flag
        private static readonly GemmaModelType[] ModelTypes = new[]
        {
            GemmaModelType.Gemma2B, GemmaModelType.Gemma2B,           // Gemma 2B
            GemmaModelType.Gemma7B, GemmaModelType.Gemma7B,           // Gemma 7B
            GemmaModelType.Griffin2B, GemmaModelType.Griffin2B,       // RecurrentGemma
            GemmaModelType.GemmaTiny,                                 // Gemma Tiny
            GemmaModelType.Gemma2_2B, GemmaModelType.Gemma2_2B,      // Gemma2 2B
            GemmaModelType.Gemma2_9B, GemmaModelType.Gemma2_9B,      // Gemma2 9B
            GemmaModelType.Gemma2_27B, GemmaModelType.Gemma2_27B,    // Gemma2 27B
            GemmaModelType.Gemma3_4B,                              // Gemma3 4B
            GemmaModelType.Gemma3_1B,                              // Gemma3 1B
            GemmaModelType.Gemma3_12B,                             // Gemma3 12B
            GemmaModelType.Gemma3_27B,
            GemmaModelType.PaliGemma224,                             // PaliGemma 224
            GemmaModelType.PaliGemma2_3B_224,                        // PaliGemma2 3B 224
            GemmaModelType.PaliGemma2_3B_448,                        // PaliGemma2 3B 448
            GemmaModelType.PaliGemma2_10B_224,                       // PaliGemma2 10B 224
            GemmaModelType.PaliGemma2_10B_448,                       // PaliGemma2 10B 448
        };

        // Cache the mapping for faster lookups
        private static readonly Dictionary<string, GemmaModelType> FlagToModelType;
        private static readonly Dictionary<GemmaModelType, List<string>> ModelTypeToFlags;

        static GemmaModelUtils()
        {
            FlagToModelType = new Dictionary<string, GemmaModelType>();
            ModelTypeToFlags = new Dictionary<GemmaModelType, List<string>>
            {
            };

            // Build the mappings
            for (int i = 0; i < ModelFlags.Length; i++)
            {
                FlagToModelType[ModelFlags[i]] = ModelTypes[i];

                if (!ModelTypeToFlags.ContainsKey(ModelTypes[i]))
                {
                    ModelTypeToFlags[ModelTypes[i]] = new List<string>();
                }
                ModelTypeToFlags[ModelTypes[i]].Add(ModelFlags[i]);
            }
        }

        public static GemmaModelType GetModelTypeFromFlag(string flag)
        {
            return FlagToModelType.TryGetValue(flag, out var modelType)
                ? modelType
                : GemmaModelType.Unknown;
        }

        public static List<string> GetFlagsForModelType(GemmaModelType modelType)
        {
            return ModelTypeToFlags.TryGetValue(modelType, out var flags)
                ? flags
                : new List<string>();
        }
    }
}