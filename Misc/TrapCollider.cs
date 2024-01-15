using AF.Health;
using UnityEngine;

namespace AF
{
    public class TrapCollider : MonoBehaviour
    {
        AudioSource audioSource => GetComponent<AudioSource>();

        [Header("Damage Settings")]
        public Damage damage;

        /// <summary>
        /// Animation Event
        /// </summary>
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
            if (!other.TryGetComponent<DamageReceiver>(out var damageReceiver))
            {
                return;
            }

            if (damage != null)
            {
                damageReceiver.TakeDamage(damage);
            }
        }

    }

}
