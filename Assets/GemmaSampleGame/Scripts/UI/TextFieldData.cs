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
using UnityEngine.UIElements;

namespace GoogleDeepMind.GemmaSampleGame.UI
{
    [CreateAssetMenu(fileName = "TextFieldData", menuName = "Gemma Sample Game/UI/Text Field Data")]
    public class TextFieldData : ScriptableObject
    {
        private string _value;
        private event System.Action<string> onValueChanged;

        public string Value => _value;

        public void SetValue(string newValue)
        {
            _value = newValue;
            onValueChanged?.Invoke(_value);
        }

        public void AddListener(System.Action<string> listener)
        {
            onValueChanged += listener;
        }

        public void RemoveListener(System.Action<string> listener)
        {
            onValueChanged -= listener;
        }
    }
}