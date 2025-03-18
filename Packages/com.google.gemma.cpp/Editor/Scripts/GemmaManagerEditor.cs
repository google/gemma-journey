using UnityEditor;
using UnityEngine;

namespace GemmaCpp.Editor
{
    [CustomEditor(typeof(GemmaManager))]
    public class GemmaManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var manager = (GemmaManager)target;

            EditorGUILayout.Space();
            if (GUILayout.Button("Create New Settings Asset"))
            {
                var settings = ScriptableObject.CreateInstance<GemmaManagerSettings>();
                var path = EditorUtility.SaveFilePanelInProject(
                    "Save Gemma Settings",
                    "GemmaSettings",
                    "asset",
                    "Please enter a file name to save the Gemma settings to"
                );

                if (!string.IsNullOrEmpty(path))
                {
                    AssetDatabase.CreateAsset(settings, path);
                    AssetDatabase.SaveAssets();
                }
            }

            DrawDefaultInspector();
        }
    }
}