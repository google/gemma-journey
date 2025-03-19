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

﻿using UnityEngine;
using UnityEditor;
using System.Collections;

public class RgbmCubedSkyboxInspector : MaterialEditor
{
    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI ();

        if (isVisible)
        {
            var material = target as Material;

            bool useLinear = false;
            foreach (var keyword in material.shaderKeywords)
            {
                if (keyword == "USE_LINEAR")
                {
                    useLinear = true;
                    break;
                }
            }

            EditorGUI.BeginChangeCheck ();

            useLinear = EditorGUILayout.Toggle("Linear Space Lighting", useLinear);

            if (EditorGUI.EndChangeCheck())
            {
                if (useLinear)
                {
                    material.EnableKeyword("USE_LINEAR");
                    material.DisableKeyword("USE_GAMMA");
                }
                else
                {
                    material.DisableKeyword("USE_LINEAR");
                    material.EnableKeyword("USE_GAMMA");
                }
                EditorUtility.SetDirty(target);
            }
        }
    }
}
