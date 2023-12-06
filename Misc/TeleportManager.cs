using System.Collections;
using UnityEngine;

namespace AF
{
    public class TeleportManager : MonoBehaviour
    {

        public string spawnGameObjectNameRef = null;

        public void Teleport(string sceneName, string spawnGameObjectNameRef)
        {
            if (string.IsNullOrEmpty(this.spawnGameObjectNameRef))
            {
                SetSpawnGameObjectNameRef(spawnGameObjectNameRef);

                //Player.instance.LoadScene((int)sceneName, true);
            }
        }

        public void SetSpawnGameObjectNameRef(string spawnGameObjectNameRef)
        {
            this.spawnGameObjectNameRef = spawnGameObjectNameRef;
        }

        public void SpawnPlayer(GameObject player)
        {
            if (string.IsNullOrEmpty(spawnGameObjectNameRef))
            {
                return;
            }

            GameObject spawnGameObject = GameObject.Find(spawnGameObjectNameRef);

            if (spawnGameObject != null)
            {
                var playerNewPos = spawnGameObject.transform.position;
                var playerNewRot = spawnGameObject.transform.rotation;

                if (spawnGameObject.transform.childCount > 0)
                {
                    var reference = spawnGameObject.gameObject.transform.GetChild(0).transform.position;

                    var rot = reference - spawnGameObject.transform.position;
                    rot.y = 0;
                    var lookRot = Quaternion.LookRotation(rot);
                    playerNewRot = lookRot;
                }

                var playerComponentManager = FindObjectOfType<PlayerComponentManager>(true);
                playerComponentManager.UpdatePosition(playerNewPos, playerNewRot);

                var activeCamera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
                if (activeCamera != null)
                {
                    var tps = FindObjectOfType<ThirdPersonController>(true);
                    tps.enabled = false;
                    activeCamera.enabled = false;
                    activeCamera.ForceCameraPosition(playerComponentManager.transform.position, playerComponentManager.transform.rotation);
                    activeCamera.PreviousStateIsValid = true;
                    activeCamera.enabled = true;
                    tps._cinemachineTargetYaw = activeCamera.transform.eulerAngles.y;
                    tps.enabled = true;
                }
            }

            this.spawnGameObjectNameRef = null;
        }
    }
}
