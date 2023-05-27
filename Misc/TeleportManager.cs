using StarterAssets;
using System.Collections;
using UnityEngine;

namespace AF
{
    public class TeleportManager: MonoBehaviour
    {
        public static TeleportManager instance;

        public string spawnGameObjectNameRef = null;

        public enum SceneName
        {
            CACILDES_HOME = 0,
            CECILY_FIELDS = 1,
            PATH_TO_SLEPBONE_PATHWAY = 2,
            THIEF_CAVERN = 3,
            SLEPBONE_BOAT = 4,
            BEAR_CAVERN = 5,
            TUTORIAL = 6,
            MOUNTAINPASS = 7,
            SNAILCLIFF = 8,
            CECILY_TOWN = 9,
            WEST_BRIDGE = 10,
            ORC_CAVE = 11,
            ORC_CAVE_2 = 12,
            ARUN_VILLAGE = 13,
            ARUN_TEMPLE_ENTRANCE = 14,
            ARUN_TEMPLE = 15
        }

        private void Awake()    
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }
        }

        public void Teleport(SceneName sceneName, string spawnGameObjectNameRef)
        {
            if (string.IsNullOrEmpty(this.spawnGameObjectNameRef))
            {
                SetSpawnGameObjectNameRef(spawnGameObjectNameRef);

                Player.instance.LoadScene((int)sceneName, true);
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
