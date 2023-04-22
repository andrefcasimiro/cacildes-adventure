using UnityEngine;

namespace AF
{
    public class ParticleSpawnManager : MonoBehaviour
    {
        public GameObject particleToSpawn;
        ClimbController player => FindObjectOfType<ClimbController>(true);
        public Transform originTransform;

        public void SpawnIntoPlayer()
        {
            Instantiate(particleToSpawn, player.playerFeetRef.transform.position, Quaternion.identity);
        }
        public void SpawnTowardsPlayer()
        {
            Vector3 pos = new Vector3(originTransform.position.x, player.playerFeetRef.transform.position.y, originTransform.position.z);
            var t = Instantiate(particleToSpawn, pos, Quaternion.identity);

            Vector3 rotDirection = player.transform.position - t.transform.position;
            var targRot = Quaternion.LookRotation(rotDirection);

            t.transform.rotation = targRot;
        }
    }

}
