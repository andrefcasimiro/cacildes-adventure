using UnityEngine;

namespace AF
{
    public class ParticleSpawnManager : MonoBehaviour
    {
        public GameObject particleToSpawn;
        public Transform originTransform;

        public bool spawnAtPlayerFeetHeight = true;

        GameObject player;

        private void Awake()
        {
            player = GameObject.FindWithTag("Player");
        }

        public void SpawnIntoPlayer()
        {
            Instantiate(particleToSpawn, spawnAtPlayerFeetHeight ? player.transform.position : player.transform.position, Quaternion.identity);
        }

        public void SpawnTowardsPlayer()
        {
            Vector3 pos = new Vector3(originTransform.position.x, spawnAtPlayerFeetHeight ? player.transform.position.y : originTransform.transform.position.y, originTransform.position.z);
            var t = Instantiate(particleToSpawn, pos, Quaternion.identity);

            Vector3 rotDirection = player.transform.position - t.transform.position;
            var targRot = Quaternion.LookRotation(rotDirection);

            t.transform.rotation = targRot;
        }
    }

}
