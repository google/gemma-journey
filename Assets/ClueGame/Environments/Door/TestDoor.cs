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
using UnityEngine.UI;

namespace GoogleDeepMind.GemmaSampleGame
{
    public class TestDoor : MonoBehaviour
    {
        [SerializeField] private DoorManager _doorManager;
        [SerializeField] private Button _openButton;
        [SerializeField] private Button _closeButton;

        private void Start()
        {
            _openButton.onClick.AddListener(() =>
            {
                _doorManager.Open();
            });
            _closeButton.onClick.AddListener(() =>
            {
                _doorManager.Close();
            });
        }
    }
}

