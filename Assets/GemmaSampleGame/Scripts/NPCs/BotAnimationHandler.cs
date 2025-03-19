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
 * BotAnimationHandler.cs
 * 
 * Manages the animations for NPC (bot) characters based on their state and interactions.
 * Controls movement animations, conversation animations (thinking, speaking, listening),
 * and reaction animations in response to player interactions.
 */
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame
{
    public class BotAnimationHandler : MonoBehaviour
{
    /** Reference to the Unity Animator component controlling this NPC */
    [SerializeField] private Animator animator;

    /**
     * Defines which side of the NPC the player is interacting from.
     * Used to trigger directional reaction animations.  
     */
    public enum InteractionSide: int
    {
        Left = -1,   // Player is to the left of the NPC
        Right = 1,    // Player is to the right of the NPC
        Direct = 0    // Player is directly in front of the NPC
    };
    
    // Animation parameter hashes for improved performance
    private readonly int moveSpeed = Animator.StringToHash("Speed");
    private readonly int isRunning = Animator.StringToHash("IsRunning");
    private readonly int isInteracting = Animator.StringToHash("IsInteracting");
    private readonly int interact = Animator.StringToHash("Interact");
    private readonly int side = Animator.StringToHash("Side");
    private readonly int think = Animator.StringToHash("Think");
    private readonly int speak = Animator.StringToHash("Speak");
    private readonly int listen = Animator.StringToHash("Listen");
    private readonly int greet = Animator.StringToHash("Greet");

    /**
     * Resets animation parameters to their default values.
     * Called before setting new animation states to prevent conflicts.
     */
    private void ResetAnimationProperties()
    {
        animator.SetBool(isRunning, false);
        animator.SetBool(isInteracting, false);
        animator.SetFloat(moveSpeed, 0);
    }
    
    /**
     * Sets the NPC to its idle animation state.
     */
    public void Idle()
    {
        Debug.Log("Bot anim idle");
        ResetAnimationProperties();
    }

    /**
     * Triggers a greeting animation, typically used when
     * the player approaches or leaves the NPC.
     */
    public void Greet()
    {
        Debug.Log("Bot greeting");
        ResetAnimationProperties();
        animator.SetTrigger(greet);
    }

    /**
     * Sets the NPC to a walking animation with the specified speed.
     * 
     * @param speed The walking speed parameter for the animation
     */
    public void Walk(float speed)
    {
        Debug.Log("Bot anim walk");
        ResetAnimationProperties();
        animator.SetFloat(moveSpeed, speed);
        animator.SetBool(isRunning, false);
    }

    /**
     * Sets the NPC to a running animation with the specified speed.
     * 
     * @param speed The running speed parameter for the animation
     */
    public void Run(float speed)
    {
        Debug.Log("Bot anim run");
        ResetAnimationProperties();
        animator.SetFloat(moveSpeed, speed);
        animator.SetBool(isRunning, true);
    }

    /**
     * Triggers a reaction animation when the NPC notices the player,
     * with the direction based on where the player is relative to the NPC.
     * 
     * @param interactionSide The side from which the interaction is occurring
     */
    public void Heard(InteractionSide interactionSide)
    {
        Debug.Log("Bot anim heard");
        ResetAnimationProperties();
        animator.SetInteger(side, (int)interactionSide);
        animator.SetBool(isInteracting, true);
        animator.SetTrigger(interact);
    }

    /**
     * Triggers the thinking animation for the NPC,
     * typically used when processing player input.
     */
    public void Think()
    {
        Debug.Log("Bot anim think");
        ResetAnimationProperties();
        animator.SetTrigger(think);
        animator.SetBool(isInteracting, true);
    }

    /**
     * Triggers the speaking animation for the NPC,
     * used when the NPC is delivering a response.
     */
    public void Speak()
    {
        Debug.Log("Bot anim speak");
        ResetAnimationProperties();
        animator.SetTrigger(speak);
        animator.SetBool(isInteracting, true);
    }

    /**
     * Triggers the listening animation for the NPC,
     * used when waiting for player input.
     */
    public void Listen()
    {
        Debug.Log("Bot anim listen");
        ResetAnimationProperties();
        animator.SetTrigger(listen);
        animator.SetBool(isInteracting, true);
    }
}
}