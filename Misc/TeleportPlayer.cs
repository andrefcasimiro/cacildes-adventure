using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class TeleportPlayer : MonoBehaviour
    {
        PlayerComponentManager playerComponentManager;
        SceneSettings sceneSettings;

        private void Awake()
        {
            sceneSettings = FindObjectOfType<SceneSettings>(true);
            playerComponentManager = FindObjectOfType<PlayerComponentManager>(true);
        }

        public void Teleport()
        {

            playerComponentManager.GetComponent<ThirdPersonController>().trackFallDamage = false;
            playerComponentManager.GetComponent<ThirdPersonController>().isSliding = false;
            playerComponentManager.GetComponent<ThirdPersonController>().isSlidingOnIce = false;
            playerComponentManager.UpdatePosition(transform.position, Quaternion.identity);
            Instantiate(sceneSettings.respawnFx, transform.position, Quaternion.identity);
            playerComponentManager.GetComponent<ThirdPersonController>().trackFallDamage = true;
        }


    }

}
