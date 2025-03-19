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

using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace GoogleDeepMind.GemmaSampleGame.UI
{
    [Serializable]
    public class TransitionHolder
    {
        [SerializeField] private StyleSheet styleSheet;
        public StyleSheet StyleSheet => styleSheet;

        public string TransitionShort => "transition-short";
        public string TransitionLong => "transition-long";
        public string Fade => "fade";
        public string SlideDownFade => "slide-down-fade";
        public string SlideRightFade => "slide-right-fade";
    }
}