using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

namespace AF
{
    [System.Serializable]
    public class SwitchEntryInstance
    {
        public SwitchEntry switchEntry;
        public bool currentValue;
    }

    public class SwitchManager : MonoBehaviour, ISaveable
    {
        public List<SwitchEntryInstance> switchEntryInstances = new List<SwitchEntryInstance>();
        
        // Event specific switch updates that are put in a queue on purpose (for when we don't want to update them immediately)
        [HideInInspector] public List<SwitchEntryInstance> queueSwitchUpdates = new List<SwitchEntryInstance>();

        public static SwitchManager instance;

        public void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
            }

            LoadSwitches();
        }

        public void LoadSwitches()
        {
            switchEntryInstances.Clear();

            var switchEntries = Resources.LoadAll<SwitchEntry>("Switches").ToList();

            foreach (var switchEntry in switchEntries)
            {
                SwitchEntryInstance switchEntryInstance = new SwitchEntryInstance();
                switchEntryInstance.switchEntry = switchEntry;
                switchEntryInstance.currentValue = false;

                switchEntryInstances.Add(switchEntryInstance);
            }
        }

        #region Update Switch Logic
        public void UpdateSwitchWithoutRefreshingEvents(SwitchEntry switchEntryToUpdate, bool nextValue)
        {
            var switchIndex = switchEntryInstances.FindIndex(switchEntry => switchEntry.switchEntry == switchEntryToUpdate);

            switchEntryInstances[switchIndex].currentValue = nextValue;

            if (nextValue == true)
            {
                HandleObjectiveUpdate(switchIndex);
            }
        }
        
        public void UpdateSwitch(SwitchEntry switchEntry, bool nextValue)
        {
            var switchIndex = switchEntryInstances.FindIndex(x => x.switchEntry == switchEntry);
            if (switchIndex == -1)
            {
                Debug.LogError("Could not find switch: " + switchEntry.name);
                return;
            }

            switchEntryInstances[switchIndex].currentValue = nextValue;

            if (nextValue == true)
            {
                HandleObjectiveUpdate(switchIndex);
            }

            NotifySwitchListeners(switchIndex);
        }

        void NotifySwitchListeners(int switchIndex)
        {
            #region Notify scene listeners in scene
            var sceneSwitchListeners = FindObjectsOfType<SwitchListener>(true);
            foreach (var sceneSwitchListener in sceneSwitchListeners)
            {
                // If switch listener is listening for this updated switch, 
                if (sceneSwitchListener.switchEntry == switchEntryInstances[switchIndex].switchEntry)
                {
                    sceneSwitchListener.Refresh();
                }
            }

            var sceneMultipleSwitchListeners = FindObjectsOfType<MultipleSwitchDependent>(true);
            foreach (var multipleSwichListener in sceneMultipleSwitchListeners)
            {
                multipleSwichListener.Refresh();
            }
            #endregion

            #region Notify events that listen to this switch in the scene
            var eventsInScene = FindObjectsOfType<Event>(true);
            foreach (var eventInScene in eventsInScene)
            {
                eventInScene.RefreshEventPages();
            }
            #endregion
        }

        void HandleObjectiveUpdate(int switchIndex)
        {
            if (switchEntryInstances[switchIndex].switchEntry.updateObjective)
            {
                var newObjective = switchEntryInstances[switchIndex].switchEntry.newObjective;
                if (!string.IsNullOrEmpty(newObjective))
                {
                    Player.instance.currentObjective = newObjective;
                }
            }
        }
        #endregion

        #region Queue Switches Logic
        public void UpdateQueuedSwitches()
        {
            if (queueSwitchUpdates.Count <= 0)
            {
                return;
            }

            foreach (SwitchEntryInstance queuedSwitch in queueSwitchUpdates)
            {
                UpdateSwitch(queuedSwitch.switchEntry, queuedSwitch.currentValue);
            }

            queueSwitchUpdates.Clear();
        }

        public void AddQueueSwitch(SwitchEntry switchEntry, bool nextValue)
        {
            var existingQueueSwitchIndex = this.queueSwitchUpdates.FindIndex(queueSwitch => queueSwitch.switchEntry == switchEntry);
            if (existingQueueSwitchIndex != -1)
            {
                this.queueSwitchUpdates[existingQueueSwitchIndex].currentValue = nextValue;
                return;
            }

            SwitchEntryInstance switchEntryInstance = new SwitchEntryInstance();
            switchEntryInstance.switchEntry = switchEntry;
            switchEntryInstance.currentValue = nextValue;
            this.queueSwitchUpdates.Add(switchEntryInstance);
        }
        #endregion

        #region Get Switch Logic
        public bool GetSwitchCurrentValue(SwitchEntry switchEntry)
        {
            var targetSwitch = switchEntryInstances.Find(switchEntryInstance => switchEntryInstance.switchEntry == switchEntry);

            return targetSwitch != null && targetSwitch.currentValue;
        }
        #endregion

        #region Serialization
        public void OnGameLoaded(GameData gameData)
        {
            if (gameData.switches.Length <= 0)
            {
                return;
            }

            foreach (SerializableSwitch savedSwitch in gameData.switches)
            {
                var switchIndex = switchEntryInstances.FindIndex(x => x.switchEntry.name == savedSwitch.switchName);
                if (switchIndex == -1)
                {
                    Debug.LogError("Could not find switch with name: " + savedSwitch.switchName);
                    return;
                }

                UpdateSwitch(switchEntryInstances[switchIndex].switchEntry, savedSwitch.value);
            }
        }
        #endregion
    }
}