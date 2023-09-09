using UnityEngine;

namespace AF
{
    public class TrapCollider : MonoBehaviour
    {
        public float moveForce = 25f;

        public float damage = 20;

        public AudioClip sound;

        AudioSource audioSource => GetComponent<AudioSource>();

        PlayerHealthbox playerHealthbox;

        public void PlaySwingSound()
        {
            // Not all trap colliders have soundsources sometimes (to not have multiple audiosources when only one is needed)
            if (this.audioSource == null)
            {
                return;
            }

            this.audioSource.Play();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("PlayerHealthbox"))
            {

                other.GetComponent<PlayerHealthbox>().TakeEnvironmentalDamage(damage, 0, false, WeaponElementType.None);
            }
            else if (other.gameObject.CompareTag("Enemy"))
            {
                EnemyManager enemyManager = other.GetComponent<EnemyManager>();
                enemyManager.enemyHealthController.TakeEnvironmentalDamage(damage);
            }
        }

    }

}
