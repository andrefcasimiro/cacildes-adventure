using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class ActivateSwitchAction : MonoBehaviour
    {
        public string switchUuid;

        public void ActivateSwitch()
        {
            SwitchManager.instance.UpdateSwitch(switchUuid, true);
        }
    }

}
