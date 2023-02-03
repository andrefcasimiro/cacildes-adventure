using System;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class SwitchManager : MonoBehaviour, ISaveable
    {
        public List<Switch> switches = new List<Switch>();

        public static SwitchManager instance;

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

            GetSwitches();
        }

        public void GetSwitches()
        {
            this.switches.Clear();
            Switch[] switches = this.transform.GetComponentsInChildren<Switch>(true);

            foreach (var _switch in switches)
            {
                this.switches.Add(_switch);
            }
        }

        public void UpdateSwitchWithoutRefreshingEvents(string switchId, bool nextValue)
        {
            var switchIndex = this.switches.FindIndex(x => x.ID == switchId);

            this.switches[switchIndex].value = nextValue;

            HandleObjectiveUpdate(switchIndex);

        }

        public void UpdateSwitch(string switchID, bool nextValue)
        {

            var switchIndex = this.switches.FindIndex(x => x.ID == switchID);
            this.switches[switchIndex].value = nextValue;

            HandleObjectiveUpdate(switchIndex);

            var sceneSwitchListeners = FindObjectsOfType<SwitchListener>(true);
            foreach (var sceneSwitchListener in sceneSwitchListeners)
            {
                // if the switch listener starts as a deactivated gameobject, we need to fix the _switch
                if (sceneSwitchListener._switch == null)
                {
                    sceneSwitchListener._switch = GetSwitchInstance(sceneSwitchListener.switchUuid);
                }

                if (sceneSwitchListener._switch.ID == switchID)
                {
                    sceneSwitchListener.EvaluateSwitch();
                }
            }

            var sceneMultipleSwitchListeners = FindObjectsOfType<MultipleSwitchDependent>(true);
            foreach (var multipleSwichListener in sceneMultipleSwitchListeners)
            {
                multipleSwichListener.EvaluateSwitch();
            }

            var eventsInScene = FindObjectsOfType<Event>(true);
            foreach (var eventInScene in eventsInScene)
            {
                eventInScene.RefreshEventPages();
            }

        }

        void HandleObjectiveUpdate(int switchIndex)
        {
            if (this.switches[switchIndex].updateObjective)
            {
                if (System.String.IsNullOrEmpty(this.switches[switchIndex].newObjective) == false)
                {
                    Player.instance.currentObjective = this.switches[switchIndex].newObjective;
                }
            }
        }

        public Switch GetSwitchInstance(string switchUUID)
        {
            return this.switches.Find(x => x.ID == switchUUID);
        }

        public bool GetSwitchValue(string switchID)
        {
            var targetSwitch = this.switches.Find(x => x.ID == switchID);

            if (targetSwitch == null)
            {
                return false;
            }


            return targetSwitch.value;
        }

        public void OnGameLoaded(GameData gameData)
        {
            if (gameData.switches.Length <= 0) { return; }

            foreach (SerializableSwitch savedSwitch in gameData.switches)
            {
                UpdateSwitch(savedSwitch.uuid, savedSwitch.value);
            }
        }

        public new void SendMessage(string switchUuid)
        {
            if (System.String.IsNullOrEmpty(switchUuid))
            {
                return;
            }

            UpdateSwitch(switchUuid, true);
        }
    }

}
