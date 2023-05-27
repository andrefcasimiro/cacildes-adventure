using UnityEngine;

namespace AF
{
    public class ParticleSpawnManager : MonoBehaviour
    {
        public GameObject particleToSpawn;
        ClimbController player;
        public Transform originTransform;

        public bool spawnAtPlayerFeetHeight = true;

        private void Awake()
        {
             player = FindObjectOfType<ClimbController>(true);
        }

        public void SpawnIntoPlayer()
        {
            Instantiate(particleToSpawn, spawnAtPlayerFeetHeight ? player.playerFeetRef.transform.position : player.transform.position, Quaternion.identity);
        }

        public void SpawnTowardsPlayer()
        {
            Vector3 pos = new Vector3(originTransform.position.x, spawnAtPlayerFeetHeight ? player.playerFeetRef.transform.position.y : originTransform.transform.position.y, originTransform.position.z);
            var t = Instantiate(particleToSpawn, pos, Quaternion.identity);

            Vector3 rotDirection = player.transform.position - t.transform.position;
            var targRot = Quaternion.LookRotation(rotDirection);

            t.transform.rotation = targRot;
        }
    }

}
