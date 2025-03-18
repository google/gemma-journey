# Codebase Summary

This codebase implements "Unity Clue Demo", a 3D murder mystery game with AI-powered NPCs using Google's Gemma language model. The game focuses on dialogue interactions and environmental exploration. Here's a comprehensive breakdown of the key components:

## Core Mechanics

### Player Systems

* **Character Movement (`ClueCharacterController.cs`):** Handles player movement using InControl for input, supporting both keyboard/mouse and gamepad. It manages walking, running, and smooth rotation with physics-based movement.

* **Camera Control (`ClueCameraController.cs`):** Implements a third-person camera that follows the player, allows zooming with the mouse wheel, and rotates with right-click. It includes collision detection to prevent clipping through walls.

* **Interaction (`ClueCharacterInteractor.cs`):** Manages player interaction with NPCs and interactive objects. It uses raycasting to detect interactable objects in the player's line of sight and handles interaction distance and angle constraints.

### AI and Dialogue Systems

* **Dialogue System (`DialogueManager.cs`, `DialogueData.cs`, `DialogueUIHandler.cs`):** Manages dialogue display using UI Toolkit, including speaker names, text, and dynamic character portraits. It uses a render texture and dedicated camera for character portraits during conversations.

* **Gemma Integration (`GemmaClient.cs`, `GemmaChatFormatter.cs`):** Provides integration with Google's Gemma language model. `GemmaClient` handles communication with the Gemma API, while `GemmaChatFormatter` formats messages according to Gemma's expected input format.

* **NPCs and Chatbots (`ClueChatbot.cs`, `ChatbotState.cs`, `BotAnimationHandler.cs`):** Implements AI-powered NPCs with conversation capabilities. `ClueChatbot` manages NPC conversation flow, `ChatbotState` tracks the NPC's conversation state (idle, thinking, listening, speaking), and `BotAnimationHandler` controls animations based on these states.

* **Personality System (`PersonalityProvider.cs`, `NPCLocalSettings.cs`, `NPCGlobalSettings.cs`):** Defines NPC personalities and behavior. It loads personality data from configuration files and provides system prompts to the Gemma model. Global settings apply to all NPCs, while local settings allow for individual customization.

### Game Architecture

* **State Machine (`GameStateMachine.cs` and related State files):** Controls the overall game flow using a robust state pattern implementation. States include:
  - `StateMenu`: Main menu display and interaction
  - `StateTutorial`: Tutorial sequence
  - `StateWalkAround`: Normal gameplay exploration
  - `StateDialogue`: NPC conversation
  - `StateChangeScene`: Scene transitions
  - `StateEndgame`: End game sequence

* **Level Management (`LevelManager.cs`, `RoomManager.cs`, `DoorManager.cs`):** Handles scene loading, room configurations, and transitions. It uses additive scene loading for smooth transitions between rooms and manages door states for room connections.

* **Dependency Injection (`LifetimeScope` folder):** Uses VContainer for dependency injection, creating a modular architecture with clear separation of concerns. `ClueGameLifetimeScope` manages game-wide dependencies, while `LevelLifetimeScope` handles level-specific dependencies.

## User Interface System

* **UI Management (`UserInterfaceManager.cs`, `UserInterface.cs`):** Centralized system for managing all UI elements using Unity's UI Toolkit. The base `UserInterface` class provides common functionality, while specialized classes handle specific UI elements.

* **UI Components:**
  - `ChatPanelUserInterface.cs`: Conversation UI with NPCs
  - `ControlOverlayUserInterface.cs`: Gameplay controls display
  - `MainMenuUserInterface.cs`: Main menu screens
  - `TutorialUserInterface.cs`: Tutorial UI elements
  - `LoadingUserInterface.cs`: Loading screens
  - `EndingUserInterface.cs`: End game UI
  - `DebugUserInterface.cs`: Development and debugging UI

## Supporting Systems

* **Input System (`Input*.cs` files in `GameState` folder):** Defines input events that trigger state transitions in the game state machine. These include user actions like interacting with NPCs, navigating doors, and menu selections.

* **Audio (`SoundManager.cs`, `SoundPlayer.cs`, `BotSoundPlayer.cs`):** Manages game audio including ambient sounds, UI sounds, and character-specific audio. `BotSoundPlayer` handles NPC voice playback during conversations.

* **Secret Management (`SecretManager.cs`):** Handles "clues" or "secrets" that NPCs can reveal during conversations, tracking which secrets have been discovered by the player.

* **Testing and Development (`ChatbotTest` folder, `DebugUI.cs`):** Tools for testing and debugging the game, particularly focused on the AI conversation system.

## Project Structure

The codebase follows a clear organization pattern:

* `/Scripts/GameState/`: State machine implementation and game states
* `/Scripts/NPCs/`: NPC behavior, AI integration, and personality systems
* `/Scripts/UserInterfaces/`: UI components and managers
* `/Scripts/LifetimeScope/`: Dependency injection setup
* `/ClueGame/`: Game assets, prefabs, and settings
* `/ChatbotTest/`: Testing tools for the conversation system

## Technical Architecture

The game employs several design patterns:

1. **State Pattern**: For game flow and NPC behavior management
2. **Dependency Injection**: Using VContainer for service location and composition
3. **Observer Pattern**: For communication between loosely coupled components
4. **Model-View-Controller**: Separation of game logic, presentation, and input

## Key Technologies

* Unity Engine with Universal Render Pipeline (URP)
* UI Toolkit for modern UI implementation
* Gemma API for AI-powered conversations
* InControl for input handling
* VContainer for dependency injection

This architecture creates a modular, maintainable codebase with clear separation of concerns, making it easy to extend with new features and content.