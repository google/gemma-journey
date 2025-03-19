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
 * BotSoundPlayer.cs
 * 
 * Plays footstep sounds for NPC characters based on their animation state.
 * This component monitors a foot step animation parameter and triggers
 * appropriate sound effects when the character's feet touch the ground.
 * 
 * Random sound clips are selected from a configurable array to add variety
 * to the footstep sounds, and they are played at the position of the
 * appropriate foot transform.
 */
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame
{
    public class BotSoundPlayer : MonoBehaviour
{
    /** Array of footstep sound clips to randomly choose from */
    [SerializeField] private AudioClip[] _audioClips;
    
    /** Reference to the character's animator component */
    [SerializeField] private Animator _animator;
    
    /** Transform representing the position of the left foot */
    [SerializeField] private Transform _leftFootTransform;
    
    /** Transform representing the position of the right foot */
    [SerializeField] private Transform _rightFootTransform;

    /** Last frame's footstep parameter value for change detection */
    private float _lastFootStep = 0;
    
    /** Animator parameter hash for the FootStep parameter */
    private int footStepHash = Animator.StringToHash("FootStep");
    
    /**
     * Checks for footstep events every frame.
     */
    private void Update()
    {
        PlayFootStep();
    }

    /**
     * Plays appropriate footstep sounds based on the animation state.
     * Detects when the FootStep parameter crosses zero in either direction,
     * indicating a foot has touched the ground, and plays a random sound
     * at the position of the corresponding foot.
     */
    private void PlayFootStep()
    {
        float footStep = _animator.GetFloat(footStepHash);
        if (Mathf.Abs(footStep) < 0.001f)
        {
            footStep = 0;
        }
        if (_lastFootStep > 0 && footStep < 0)
        {
            AudioClip randomClip = _audioClips[Random.Range(0, _audioClips.Length)];
            AudioSource.PlayClipAtPoint(randomClip, _leftFootTransform.position);
        }
        if (_lastFootStep < 0 && footStep > 0)
        {
            AudioClip randomClip = _audioClips[Random.Range(0, _audioClips.Length)];
            AudioSource.PlayClipAtPoint(randomClip, _rightFootTransform.position);
        }
        _lastFootStep = footStep;
    }
}
}
