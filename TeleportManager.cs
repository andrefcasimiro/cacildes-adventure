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

        public void Teleport(string sceneName, string spawnGameObjectNameRef)
        {
            SetSpawnGameObjectNameRef(spawnGameObjectNameRef);

            SceneManager.LoadScene(sceneName);
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
                player.transform.position = spawnGameObject.transform.position;
                player.transform.rotation = spawnGameObject.transform.rotation;

                if (spawnGameObject.gameObject.transform.childCount > 0)
                {
                    var reference = spawnGameObject.gameObject.transform.GetChild(0).transform.position;

                    var rot = reference - player.transform.position;
                    rot.y = 0;
                    var lookRot = Quaternion.LookRotation(rot);
                    player.transform.rotation = lookRot;
                }

                this.spawnGameObjectNameRef = null;
            }
        }
    }

}
