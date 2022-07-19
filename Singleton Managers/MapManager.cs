using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF
{
    public class MapManager: MonoBehaviour
    {
        public static MapManager instance;

        public string spawnGameObjectNameRef = "Initial Spawnpoint";

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

                this.spawnGameObjectNameRef = null;
            }
        }
    }

}
