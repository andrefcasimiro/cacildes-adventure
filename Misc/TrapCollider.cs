using UnityEngine;

namespace AF
{
    public class TrapCollider : MonoBehaviour
    {
        public float moveForce = 25f;

        public float damage = 20;

        public AudioClip sound;

        AudioSource audioSource => GetComponent<AudioSource>();

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
            if (other.gameObject.tag == "Player")
            {
                other.GetComponentInChildren<PlayerHealthbox>(true).TakeEnvironmentalDamage(damage, 0, false);
            }

            if (other.gameObject.tag == "Enemy")
            {
                other.GetComponent<EnemyManager>().enemyHealthController.TakeEnvironmentalDamage(damage);

                var rigidBody = other.GetComponent<Rigidbody>();
                if (rigidBody != null)
                {
                    rigidBody.AddForce(moveForce * other.ClosestPoint(other.transform.position), ForceMode.Acceleration);
                }

            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                other.GetComponent<ImpactReceiver>().AddImpact(other.ClosestPointOnBounds(transform.position) * moveForce, moveForce);
            }

        }

    }

}
