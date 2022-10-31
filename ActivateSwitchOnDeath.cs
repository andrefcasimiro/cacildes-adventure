using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class ActivateSwitchOnDeath : MonoBehaviour
    {
        public Switch switchToActivate;

        public void ActivateSwitch()
        {
            SwitchManager.instance.UpdateSwitch(switchToActivate.name, true);

            // Refresh all event pages in the scene
            Event[] events = FindObjectsOfType<Event>();

            foreach (Event ev in events)
            {
                ev.RefreshEventPages();
            }
        }
    }

}
