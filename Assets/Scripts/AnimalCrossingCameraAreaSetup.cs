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

/**
 * AnimalCrossingCameraAreaSetup.cs
 * 
 * This script demonstrates how to set up a special camera area for the Animal Crossing camera system.
 * It creates a trigger volume that can be used to define areas with custom camera behavior.
 */
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame
{
    [RequireComponent(typeof(Collider))]
    public class AnimalCrossingCameraAreaSetup : MonoBehaviour
    {
        [Header("Area Settings")]
        [SerializeField] private string areaName = "Special Camera Area";
        [SerializeField] private Color gizmoColor = new Color(0.2f, 0.8f, 0.2f, 0.3f);

        [Header("Camera Behavior")]
        [SerializeField] private bool overrideIndoorOutdoor = true;
        [SerializeField] private bool treatAsIndoor = true;
        [SerializeField] private bool useCustomCameraPitch = false;
        [SerializeField] private float customCameraPitch = 40f;

        private SpecialCameraArea specialCameraArea;
        private Collider triggerCollider;

        private void Awake()
        {
            // Ensure we have a collider and it's set as a trigger
            triggerCollider = GetComponent<Collider>();
            if (triggerCollider != null)
            {
                triggerCollider.isTrigger = true;
            }
            else
            {
                Debug.LogError("AnimalCrossingCameraAreaSetup: No collider found on this GameObject!");
            }

            // Add the SpecialCameraArea component if it doesn't exist
            specialCameraArea = GetComponent<SpecialCameraArea>();
            if (specialCameraArea == null)
            {
                specialCameraArea = gameObject.AddComponent<SpecialCameraArea>();
            }

            // Configure the special camera area
            ConfigureSpecialArea();
        }

        /// <summary>
        /// Configures the special camera area with the settings from this component
        /// </summary>
        private void ConfigureSpecialArea()
        {
            if (specialCameraArea != null)
            {
                specialCameraArea.areaName = areaName;
                specialCameraArea.overrideIndoorOutdoor = overrideIndoorOutdoor;
                specialCameraArea.treatAsIndoor = treatAsIndoor;
                specialCameraArea.useCustomCameraPitch = useCustomCameraPitch;
                specialCameraArea.customCameraPitch = customCameraPitch;
            }
        }

        /// <summary>
        /// Draws gizmos in the editor to visualize the special camera area
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;

            // Draw a box or sphere based on the collider type
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                if (col is BoxCollider)
                {
                    BoxCollider boxCol = col as BoxCollider;
                    Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.DrawCube(boxCol.center, boxCol.size);
                }
                else if (col is SphereCollider)
                {
                    SphereCollider sphereCol = col as SphereCollider;
                    Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.DrawSphere(sphereCol.center, sphereCol.radius);
                }
                else
                {
                    // For other collider types, just draw a wire cube around the bounds
                    Gizmos.DrawWireCube(transform.position, Vector3.one);
                }
            }

            // Draw the area name
#if UNITY_EDITOR
            UnityEditor.Handles.color = Color.white;
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, areaName);
#endif
        }

        /// <summary>
        /// Validates the component settings when values are changed in the inspector
        /// </summary>
        private void OnValidate()
        {
            // Update the special area configuration when inspector values change
            if (specialCameraArea != null)
            {
                ConfigureSpecialArea();
            }

            // Ensure the collider is set as a trigger
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }
    }
}