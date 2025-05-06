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