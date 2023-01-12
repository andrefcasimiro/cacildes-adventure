using AF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleSwitchDependent : MonoBehaviour, ISaveable
{
    [System.Serializable]
    public class SwitchEntry
    {
        public string switchUuid;
        public string description;
        public bool requiredValue;
    }

    public SwitchEntry[] switches;

    private void Start()
    {
        EvaluateSwitch();
    }

    public void OnGameLoaded(GameData gameData)
    {
        EvaluateSwitch();
    }

    public void EvaluateSwitch()
    {
        bool childrenAreActive = false;

        foreach (var switchEntry in switches)
        {
            if (SwitchManager.instance.GetSwitchValue(switchEntry.switchUuid) == switchEntry.requiredValue)
            {
                childrenAreActive = true;
                continue;
            }

            childrenAreActive = false;
            break;
        }

        transform.GetChild(0).gameObject.SetActive(childrenAreActive);
    }

}
