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
using VContainer;
using VContainer.Unity;
using GemmaCpp;


namespace GoogleDeepMind.GemmaSampleGame
{
    /**
     * This is a VContainer lifetime scope for the ChatbotTest scene.
     */
    public class ChatbotTestLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private GemmaManagerSettings gemmaSettings;

        [SerializeField]
        private ChatbotForm chatbotForm;

        protected override void Configure(IContainerBuilder builder)
        {
            // Create and configure GemmaManager
            var gemmaManagerObj = new GameObject("GemmaManager");
            var gemmaManager = gemmaManagerObj.AddComponent<GemmaManager>();
            gemmaManager.settings = gemmaSettings;
            DontDestroyOnLoad(gemmaManagerObj);

            // Register GemmaManager for injection
            builder.RegisterComponent(gemmaManager);

            // Register ChatbotForm for injection
            builder.RegisterComponent(chatbotForm);
        }

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }

    }
}