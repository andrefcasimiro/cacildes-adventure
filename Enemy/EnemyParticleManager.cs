using UnityEngine;

namespace AF
{
    public class EnemyParticleManager : MonoBehaviour
    {
        public GameObject[] particleList;

        private void Awake()
        {
            FinishParticle();
        }

        public void LaunchParticle()
        {
            var chance = Random.Range(0, particleList.Length);

            particleList[chance].gameObject.SetActive(true);
        }

        public void FinishParticle()
        {
            foreach (var particle in particleList)
            {
                particle.gameObject.SetActive(false);
            }
        }
    }
}
