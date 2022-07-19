using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class ObjectPooler : MonoBehaviour
    {
        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        public static ObjectPooler instance;

        public List<Pool> pools;
        public Dictionary<string, Queue<GameObject>> poolDictionary;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);

                    objectPool.Enqueue(obj);
                }

                poolDictionary.Add(pool.tag, objectPool);
            }
        }

        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            GameObject objectToSpawn = poolDictionary[tag].Dequeue();

            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            poolDictionary[tag].Enqueue(objectToSpawn);

            return objectToSpawn;
        }


        public void SpawnFromPool(string tag, Vector3 position, Quaternion rotation, float deactivateAfter)
        {
            GameObject objectToSpawn = poolDictionary[tag].Dequeue();

            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            poolDictionary[tag].Enqueue(objectToSpawn);

            StartCoroutine(DeactivateAfter(objectToSpawn, deactivateAfter));
        }

        IEnumerator DeactivateAfter(GameObject objectToDeactivate, float yieldTime)
        {
            yield return new WaitForSeconds(yieldTime);

            objectToDeactivate.SetActive(false);
        }

    }
}