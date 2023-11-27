using AF;
using UnityEngine;

public class MultipleConditionalSwitchDependent : MonoBehaviour
{
    [System.Serializable]
    public class MultipleSwitchEntry
    {
        public SwitchEntry switchEntry;
        public bool requiredValue;
    }

    public MultipleSwitchEntry[] switches;

    [TextArea]
    public string comment = "It will activate if one of the switch values is true";

    private void Start()
    {
        Refresh();
    }

    public void OnGameLoaded(object gameData)
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
                break;
            }
        }

        transform.GetChild(0).gameObject.SetActive(childrenAreActive);
    }
}
