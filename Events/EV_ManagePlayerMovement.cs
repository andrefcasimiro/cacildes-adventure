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
                playerComponentManager.EnableCharacterController();
            }
            else
            {
                playerComponentManager.DisableComponents();
                playerComponentManager.DisableCharacterController();
            }

            yield return null;
        }
    }
}