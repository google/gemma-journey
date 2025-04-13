// Copyright 2025 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using UnityEditor;
using System.Collections;

public class GradientSkyboxInspector : MaterialEditor
{
    public override void OnInspectorGUI ()
    {
        serializedObject.Update ();

        if (isVisible)
        {
            EditorGUI.BeginChangeCheck ();

            ColorProperty (GetMaterialProperty (targets, "_Color2"), "Top Color");
            ColorProperty (GetMaterialProperty (targets, "_Color1"), "Bottom Color");
            FloatProperty (GetMaterialProperty (targets, "_Intensity"), "Intensity");
            FloatProperty (GetMaterialProperty (targets, "_Exponent"), "Exponent");

            var dp = GetMaterialProperty (targets, "_UpVectorPitch");
            var dy = GetMaterialProperty (targets, "_UpVectorYaw");

            if (dp.hasMixedValue || dy.hasMixedValue)
            {
                EditorGUILayout.HelpBox ("Editing angles is disabled because they have mixed values.", MessageType.Warning);
            }
            else
            {
                FloatProperty (dp, "Pitch");
                FloatProperty (dy, "Yaw");
            }

            if (EditorGUI.EndChangeCheck ())
            {
                var rp = dp.floatValue * Mathf.Deg2Rad;
                var ry = dy.floatValue * Mathf.Deg2Rad;
                
                var upVector = new Vector4 (
                    Mathf.Sin (rp) * Mathf.Sin (ry),
                    Mathf.Cos (rp),
                    Mathf.Sin (rp) * Mathf.Cos (ry),
                    0.0f
                );
                GetMaterialProperty (targets, "_UpVector").vectorValue = upVector;

                PropertiesChanged ();
            }
        }
    }
}
