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
 * SoundPlayer.cs
 * 
 * Simple component for playing a single audio clip at the object's position.
 * This utility class provides a straightforward way to attach and trigger
 * sound effects from game objects via inspector configuration or script calls.
 */
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame
{
    public class SoundPlayer : MonoBehaviour
    {
        /** The audio clip to play when triggered */
        [SerializeField] private AudioClip _audioClip;

        /**
         * Plays the configured audio clip at this object's position.
         * Uses Unity's PlayClipAtPoint for simple one-shot audio playback
         * without needing to configure a full AudioSource component.
         */
        public void PlaySound()
        {
            AudioSource.PlayClipAtPoint(_audioClip, transform.position);
        }
    }
}