using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF
{
    [System.Serializable]
    public class RegisteredLocalSwitch
    {
        public string uuid;
        public LocalSwitchName localSwitchName;
    }

    public class LocalSwitchManager : MonoBehaviour, ISaveable
    {
        [SerializeField]
        public List<RegisteredLocalSwitch> registeredLocalSwitches = new List<RegisteredLocalSwitch>();

        public static LocalSwitchManager instance;

        public void Awake()
        {

            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }
        }

        protected void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            LocalSwitch[] localSwitchesInScene = FindObjectsOfType<LocalSwitch>(true);

            this.GatherLocalSwitchesInActiveScene(localSwitchesInScene);
        }

        /// <summary>
        ///  Collects local switches across scenes
        /// </summary>
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            LocalSwitch[] localSwitchesInScene = FindObjectsOfType<LocalSwitch>(true);

            this.GatherLocalSwitchesInActiveScene(localSwitchesInScene);

            GameData gameData = SaveSystem.instance.GetGameData();

            // Load local switch values in memory for the given scene
            foreach (LocalSwitch localSwitchInScene in localSwitchesInScene)
            {
                RegisteredLocalSwitch registeredLocalSwitch = this.registeredLocalSwitches.Find(registeredLocalSwitch => registeredLocalSwitch.uuid == localSwitchInScene.ID);
                if (registeredLocalSwitch != null)
                {

                    UpdateLocalSwitch(localSwitchInScene.ID, registeredLocalSwitch.localSwitchName);
                }
            }
        }

        void GatherLocalSwitchesInActiveScene(LocalSwitch[] localSwitchesInScene)
        {
            foreach (LocalSwitch localSwitchInScene in localSwitchesInScene)
            {
                RegisteredLocalSwitch registeredLocalSwitch = this.registeredLocalSwitches.Find(registeredLocalSwitch => registeredLocalSwitch.uuid == localSwitchInScene.ID);

                // Add new entry if not registered
                if (registeredLocalSwitch == null)
                {
                    RegisteredLocalSwitch newRegisteredLocalSwitch = new RegisteredLocalSwitch();
                    newRegisteredLocalSwitch.uuid = localSwitchInScene.ID;
                    newRegisteredLocalSwitch.localSwitchName = localSwitchInScene.localSwitchName;
                    this.registeredLocalSwitches.Add(newRegisteredLocalSwitch);
                }
            }
        }

        public void OnGameLoaded(GameData gameData)
        {
            // Empty local switches on the scene!
            foreach (SerializableLocalSwitch savedLocalSwitch in gameData.localSwitches)
            {
                Enum.TryParse(savedLocalSwitch.localSwitchName, out LocalSwitchName nextLocalSwitchName);

                UpdateLocalSwitch(savedLocalSwitch.uuid, nextLocalSwitchName);
            }
        }

        /// <summary>
        /// Used by both the scene change event and the load game event
        /// </summary>
        public void UpdateLocalSwitch(string uuid, LocalSwitchName nextLocalSwitchName)
        {
            var targetLocalSwitchIdx = this.registeredLocalSwitches.FindIndex(registeredLocalSwitch => registeredLocalSwitch.uuid == uuid);
            if (targetLocalSwitchIdx != -1)
            {
                this.registeredLocalSwitches[targetLocalSwitchIdx].localSwitchName = nextLocalSwitchName;
            }

            // Update local switch in scene, if available. If not, we already have its value stored in memory for when we visit the scene.
            LocalSwitch[] localSwitchesInScene = FindObjectsOfType<LocalSwitch>(true);
            LocalSwitch targetLocalSwitch = localSwitchesInScene.FirstOrDefault(localSwitchInScene => localSwitchInScene.ID == uuid);

            // If local switch is not in scene, do not update it
            if (targetLocalSwitch == null)
            {
                return;
            }

            targetLocalSwitch.UpdateLocalSwitchValue(nextLocalSwitchName);
        }

        public void UpdateLocalSwitchDatabaseEntry(LocalSwitch localSwitchToUpdate)
        {
            var idx = this.registeredLocalSwitches.FindIndex(registeredLocalSwitch => registeredLocalSwitch.uuid == localSwitchToUpdate.ID);
            if (idx == -1)
            {
                RegisteredLocalSwitch registeredLocalSwitch = new RegisteredLocalSwitch();
                registeredLocalSwitch.uuid = localSwitchToUpdate.ID;
                registeredLocalSwitch.localSwitchName = localSwitchToUpdate.localSwitchName;
                this.registeredLocalSwitches.Add(registeredLocalSwitch);
            }
            else
            {
                this.registeredLocalSwitches[idx].localSwitchName = localSwitchToUpdate.localSwitchName;
            }

        }
    }

}
