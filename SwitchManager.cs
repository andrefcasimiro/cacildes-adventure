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
            this.switches.Find(x => x.ID == switchId).value = nextValue;
        }

        public void UpdateSwitch(string switchID, bool nextValue)
        {
            this.switches.Find(x => x.ID == switchID).value = nextValue;

            var sceneSwitchListeners = FindObjectsOfType<SwitchListener>(true);
            foreach (var sceneSwitchListener in sceneSwitchListeners)
            {
                if (sceneSwitchListener._switch.ID == switchID)
                {
                    sceneSwitchListener.EvaluateSwitch();
                }
            }

            var eventsInScene = FindObjectsOfType<Event>(true);
            foreach (var eventInScene in eventsInScene)
            {
                eventInScene.RefreshEventPages();
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
    }

}
