using AF;
using UnityEngine;

public class MultipleSwitchDependent : MonoBehaviour, ISaveable
{
    [System.Serializable]
    public class MultipleSwitchEntry
    {
        public SwitchEntry switchEntry;
        public bool requiredValue;
    }

    public MultipleSwitchEntry[] switches;

    private void Start()
    {
        Refresh();
    }

    public void OnGameLoaded(GameData gameData)
    {
        Refresh();
    }

    public void Refresh()
    {
        bool childrenAreActive = false;

        foreach (var _switch in switches)
        {
            if (SwitchManager.instance.GetSwitchCurrentValue(_switch.switchEntry) == _switch.requiredValue)
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
