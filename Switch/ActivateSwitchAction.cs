using UnityEngine;

namespace AF
{
    public class ActivateSwitchAction : MonoBehaviour
    {
        public SwitchEntry switchEntry;

        public void ActivateSwitch()
        {
            SwitchManager.instance.UpdateSwitch(switchEntry, true, null);
        }
    }
}
