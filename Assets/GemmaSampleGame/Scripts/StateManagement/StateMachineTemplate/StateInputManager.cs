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
 * StateInputManager.cs
 * 
 * Manages input events for the state machine system. This class collects, buffers,
 * and provides access to state change inputs that drive transitions between states.
 * 
 * The manager uses a double-buffering system to ensure that inputs are processed
 * consistently across frames, with a clear separation between the current frame's
 * inputs and the next frame's inputs.
 */
using System.Collections.Generic;
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
  public class StateInputManager
  {
    /** Double buffer for inputs (current frame and next frame) */
    private List<List<StateInput>> _inputs;

    /** Special cache for inputs that triggered a state change */
    private List<StateInput> _stateChangeInputs;

    /** Index of the buffer currently being written to */
    private int _inputIndex;

    /** Index of the buffer currently being read from */
    private int _lastInputIndex;

    /**
     * Initializes the input manager with double buffer for inputs.
     */
    public StateInputManager()
    {
      _inputs = new List<List<StateInput>>();
      _inputs.Add(new List<StateInput>());
      _inputs.Add(new List<StateInput>());
      _stateChangeInputs = new List<StateInput>();
      _inputIndex = 0;
      _lastInputIndex = 0;
    }

    /**
     * Adds a new input to the current input buffer.
     * 
     * @param input The input to add
     */
    public void AddInput(StateInput input)
    {
      Debug.Log($"Input added: {input.GetType()}\n{input}");
      _inputs[_inputIndex].Add(input);
    }

    /**
     * Captures the current set of inputs as state change inputs.
     * Usually called when a state change is about to occur to preserve
     * the inputs that triggered the change.
     */
    public void SetStateChangeInputs()
    {
      _stateChangeInputs.Clear();
      _stateChangeInputs.AddRange(GetInputs());
    }

    /**
     * Checks if any inputs of the specified type exist in the current buffer.
     * 
     * @return True if at least one input of the specified type exists
     */
    public bool HasInput<InputType>() where InputType : StateInput
    {
      return HasInput<InputType>(GetInputs());
    }

    /**
     * Checks if any inputs of the specified type exist in the state change cache.
     * 
     * @return True if at least one input of the specified type exists in the state change cache
     */
    public bool HasStateChangeInput<InputType>() where InputType : StateInput
    {
      return HasInput<InputType>(_stateChangeInputs);
    }

    /**
     * Static helper to check if any inputs of the specified type exist in a given list.
     * 
     * @param inputs The list of inputs to check
     * @return True if at least one input of the specified type exists
     */
    public static bool HasInput<InputType>(List<StateInput> inputs) where InputType : StateInput
    {
      return inputs.Exists((StateInput input) =>
      {
        return input.GetType() == typeof(InputType);
      });
    }

    /**
     * Gets the first input of the specified type from the current buffer.
     * 
     * @return The first input of the specified type, or null if none exists
     */
    public InputType GetFirstInput<InputType>() where InputType : StateInput
    {
      return GetFirstInput<InputType>(GetInputs());
    }

    /**
     * Gets the first input of the specified type from the state change cache.
     * 
     * @return The first state change input of the specified type, or null if none exists
     */
    public InputType GetFirstStateChangeInput<InputType>() where InputType : StateInput
    {
      return GetFirstInput<InputType>(_stateChangeInputs);
    }

    /**
     * Static helper to get the first input of the specified type from a given list.
     * 
     * @param inputs The list of inputs to search
     * @return The first input of the specified type, or null if none exists
     */
    public static InputType GetFirstInput<InputType>(List<StateInput> inputs) where InputType : StateInput
    {
      return StateInput.MyConvertTo<InputType>(inputs.Find((StateInput input) =>
      {
        return input.GetType() == typeof(InputType);
      }));
    }

    /**
     * Gets the last input of the specified type from the current buffer.
     * 
     * @return The last input of the specified type, or null if none exists
     */
    public InputType GetLastInput<InputType>() where InputType : StateInput
    {
      return GetLastInput<InputType>(GetInputs());
    }

    /**
     * Gets the last input of the specified type from the state change cache.
     * 
     * @return The last state change input of the specified type, or null if none exists
     */
    public InputType GetLastStateChangeInput<InputType>() where InputType : StateInput
    {
      return GetLastInput<InputType>(_stateChangeInputs);
    }

    /**
     * Static helper to get the last input of the specified type from a given list.
     * 
     * @param inputs The list of inputs to search
     * @return The last input of the specified type, or null if none exists
     */
    public static InputType GetLastInput<InputType>(List<StateInput> inputs) where InputType : StateInput
    {
      return StateInput.MyConvertTo<InputType>(inputs.FindLast((StateInput input) =>
      {
        return input.GetType() == typeof(InputType);
      }));
    }

    /**
     * Gets all inputs from the current buffer.
     * 
     * @return The list of all current inputs
     */
    public List<StateInput> GetInputs()
    {
      return _inputs[_lastInputIndex];
    }

    /**
     * Gets all inputs of the specified type from the current buffer.
     * 
     * @return A list of all inputs of the specified type
     */
    public List<InputType> GetInputs<InputType>() where InputType : StateInput
    {
      return GetInputs<InputType>(GetInputs());
    }

    /**
     * Gets all inputs of the specified type from the state change cache.
     * 
     * @return A list of all state change inputs of the specified type
     */
    public List<InputType> GetStateChangeInputs<InputType>() where InputType : StateInput
    {
      return GetInputs<InputType>(_stateChangeInputs);
    }

    /**
     * Static helper to get all inputs of the specified type from a given list.
     * 
     * @param inputs The list of inputs to filter
     * @return A list of all inputs of the specified type
     */
    public static List<InputType> GetInputs<InputType>(List<StateInput> inputs) where InputType : StateInput
    {
      return inputs.FindAll(delegate (StateInput input)
      {
        return input.GetType() == typeof(InputType);
      }).ConvertAll(StateInput.MyConvertTo<InputType>);
    }

    /**
     * Swaps the input buffers, preparing for the next frame.
     * The current read buffer becomes the write buffer for the next frame,
     * and is cleared in the process.
     */
    public void SwapInput()
    {
      _lastInputIndex = _inputIndex;
      _inputIndex = (_inputIndex + 1) % _inputs.Count;
      _inputs[_inputIndex].Clear();
    }
  }
}