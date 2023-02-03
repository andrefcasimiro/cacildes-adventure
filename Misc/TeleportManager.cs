using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            if (System.String.IsNullOrEmpty(this.spawnGameObjectNameRef))
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
            if (System.String.IsNullOrEmpty(spawnGameObjectNameRef))
            {
                return;
            }

            GameObject spawnGameObject = GameObject.Find(spawnGameObjectNameRef);

            if (spawnGameObject != null)
            {


                var playerNewPos = spawnGameObject.transform.position;
                var playerNewRot = spawnGameObject.transform.rotation;

                if (spawnGameObject.gameObject.transform.childCount > 0)
                {
                    var reference = spawnGameObject.gameObject.transform.GetChild(0).transform.position;

                    var rot = reference - player.transform.position;
                    rot.y = 0;
                    var lookRot = Quaternion.LookRotation(rot);
                    playerNewRot = lookRot;
                }

                FindObjectOfType<PlayerComponentManager>(true).UpdatePosition(playerNewPos, playerNewRot);
            }

            this.spawnGameObjectNameRef = null;
        }
    }

}
