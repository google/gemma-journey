/**
 * LevelManager.cs
 * 
 * This class manages the game's level system, handling scene loading, transitions,
 * and room connections. It maintains references to all available levels and their types
 * (regular levels, start scene, ending scene), and controls the progression between them.
 * 
 * The manager uses Unity's scene management system with additive scene loading to create
 * seamless transitions between connected rooms and levels. It also handles the positioning
 * of rooms to ensure proper alignment at connection points.
 */
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace GoogleDeepMind.GemmaSampleGame
{
    [Serializable]
    public class LevelManager
    {
        /// <summary>
        /// Defines the different types of levels in the game.
        /// </summary>
        public enum LevelType
        {
            Level,  // Regular gameplay level
            Ending, // End game sequence
            Start   // Main menu or starting area
        }

        /// <summary>
        /// Data structure for storing information about each level.
        /// </summary>
        [Serializable]
        public class LevelData
        {
            public SceneField Scene;  // Reference to the scene asset
            public LevelType Type;    // Type of level
            public bool Enabled = true; // Whether this level is active in the current game
        }
        [SerializeField] private List<LevelData> levelDatas;

        [Inject]
        StateManagement.StateInputManager stateInputManager;
        private LifetimeScope lifetimeScope;

        private int currentLevelIndex = -1;
        private int nextLevelIndex = -1;

        private LevelData startScene;
        private List<LevelData> levelScenes;
        private LevelData endingScene;

        private RoomManager nextRoomManager;
        private RoomManager currentRoomManager;

        public RoomManager CurrentRoomManager => currentRoomManager;

        /// <summary>
        /// Initializes the level manager by categorizing levels and loading the start scene.
        /// Called when the game begins.
        /// </summary>
        public async void Init()
        {
            levelScenes = new List<LevelData>();
            foreach (LevelData level in levelDatas)
            {
                if (!level.Enabled)
                {
                    continue;
                }
                switch (level.Type)
                {
                    case LevelType.Level:
                        levelScenes.Add(level);
                        break;
                    case LevelType.Ending:
                        endingScene = level;
                        break;
                    case LevelType.Start:
                        startScene = level;
                        break;
                }
            }
            currentLevelIndex = -1;
            Scene start = SceneManager.GetSceneByName(startScene.Scene);
            if (!start.IsValid())
            {
                await SceneManager.LoadSceneAsync(startScene.Scene, LoadSceneMode.Additive);
            }
        }

        /// <summary>
        /// Registers the parent lifetime scope for dependency injection in loaded scenes.
        /// </summary>
        /// <param name="lifetimeScope">The parent lifetime scope to use for dependency injection</param>
        public void RegisterLifetimeScope(LifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
        }

        /// <summary>
        /// Determines the type of the next level based on current progression.
        /// </summary>
        /// <returns>The type of the next level to load</returns>
        public LevelType GetNextLevelType()
        {
            if (currentLevelIndex < levelScenes.Count - 1)
            {
                return LevelType.Level;
            }
            else if (currentLevelIndex == levelScenes.Count - 1)
            {
                return LevelType.Ending;
            }
            else
            {
                return LevelType.Start;
            }
        }

        /// <summary>
        /// Loads the next appropriate level based on current game progression.
        /// If no next level is available, does nothing.
        /// </summary>
        public void LoadNextLevel()
        {
            LevelType levelType = GetNextLevelType();
            switch (levelType)
            {
                case LevelType.Level:
                    LoadLevel(currentLevelIndex + 1);
                    break;
                case LevelType.Ending:
                    LoadEndScene();
                    break;
                case LevelType.Start:
                    LoadStartScene();
                    break;
            }
        }

        /// <summary>
        /// Loads a specific level by index.
        /// Uses additive scene loading to maintain the current scene while loading the new one.
        /// </summary>
        /// <param name="level">Index of the level to load</param>
        public async void LoadLevel(int level)
        {
            if (level >= levelScenes.Count)
            {
                Debug.LogError("Try to load level that not exist");
                return;
            }
            Debug.Log("Loading scene");
            using (LifetimeScope.EnqueueParent(lifetimeScope))
            {
                await SceneManager.LoadSceneAsync(levelScenes[level].Scene, LoadSceneMode.Additive);
            }

            Debug.Log("Done load scene");
            nextLevelIndex = level;
            stateInputManager.AddInput(new StateManagement.InputLevelLoaded(LevelType.Level));
        }

        /// <summary>
        /// Loads the start scene, unloading all other scenes first.
        /// Used when returning to the main menu or restarting the game.
        /// </summary>
        public async void LoadStartScene()
        {
            currentRoomManager = null;
            nextRoomManager = null;
            // Unload old start scene if existed
            Scene loadedStartScene = SceneManager.GetSceneByName(startScene.Scene);
            if (loadedStartScene != null && loadedStartScene.IsValid())
            {
                await SceneManager.UnloadSceneAsync(loadedStartScene);
            }

            // Unload all level scenes
            foreach (var level in levelScenes)
            {
                Scene scene = SceneManager.GetSceneByName(level.Scene);
                if (scene.IsValid())
                {
                    await SceneManager.UnloadSceneAsync(scene);
                }
            }
            // Unload end scene
            Scene loadedEndScene = SceneManager.GetSceneByName(endingScene.Scene);
            if (loadedEndScene != null && loadedEndScene.IsValid())
            {
                await SceneManager.UnloadSceneAsync(loadedEndScene);
            }

            currentLevelIndex = -1;
            // Reload start scene 
            using (LifetimeScope.EnqueueParent(lifetimeScope))
            {
                await SceneManager.LoadSceneAsync(startScene.Scene, LoadSceneMode.Additive);
            }

            stateInputManager.AddInput(new StateManagement.InputLevelLoaded(LevelType.Start));
        }

        /// <summary>
        /// Loads the ending scene when the player completes all levels.
        /// </summary>
        public async void LoadEndScene()
        {
            using (LifetimeScope.EnqueueParent(lifetimeScope))
            {
                await SceneManager.LoadSceneAsync(endingScene.Scene, LoadSceneMode.Additive);
            }
            nextLevelIndex = levelScenes.Count;
            stateInputManager.AddInput(new StateManagement.InputLevelLoaded(LevelType.Ending));
        }

        /// <summary>
        /// Registers a room manager with the level manager and positions it correctly
        /// relative to the current room if one exists.
        /// </summary>
        /// <param name="roomManager">The room manager to register</param>
        public void RegisterRoom(RoomManager roomManager)
        {
            lifetimeScope.Container.Inject(roomManager);

            Debug.Log($"Room registered: {roomManager.name}");
            if (currentRoomManager == null)
            {
                currentLevelIndex = -1;
                currentRoomManager = roomManager;
                currentRoomManager.EnableRoom();
                return;
            }
            Vector3 offset = currentRoomManager.BackAnchor.position - roomManager.FrontAnchor.position;
            //Debug.Log($"Room registered\n new room parent position: {roomManager.transform.parent.position}\n new room front anchor position: {roomManager.FrontAnchor.transform.position}\nOld room back anchor position: {roomManager.BackAnchor.transform.position}\nOffset: {offset}");
            roomManager.transform.parent.position = roomManager.transform.position + offset;
            nextRoomManager = roomManager;
            nextRoomManager.DisableRoom();
        }

        /// <summary>
        /// Changes the active level from the current one to the next one.
        /// Disables the current room, enables the next room, and loads the next level.
        /// </summary>
        public void ChangeLevel()
        {
            currentRoomManager.DisableRoom();
            currentRoomManager = nextRoomManager;
            currentLevelIndex = nextLevelIndex;
            currentRoomManager.EnableRoom();
            LoadNextLevel();
        }
    }
}
