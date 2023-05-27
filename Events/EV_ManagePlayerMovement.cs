using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_ManagePlayerMovement : EventBase
    {
        PlayerComponentManager playerComponentManager;

        public bool enablePlayerMovement;

        private void Awake()
        {
            playerComponentManager = FindObjectOfType<PlayerComponentManager>(true);
        }

        public override IEnumerator Dispatch()
        {
            if (enablePlayerMovement)
            {
                playerComponentManager.EnableComponents();
            }
            else
            {
                playerComponentManager.DisableComponents();
            }

            yield return null;
        }
    }
}