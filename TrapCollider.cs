using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

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
                other.GetComponentInChildren<PlayerHealthbox>(true).TakeEnvironmentalDamage(damage);
            }

            if (other.gameObject.tag == "Enemy")
            {
                other.GetComponent<EnemyHealthController>().TakeEnvironmentalDamage(damage);

                var groundCheck = other.GetComponentInChildren<EnemyGroundCheck>();
                if (groundCheck != null)
                {
                    groundCheck.ApplyForce(moveForce * other.transform.position);
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
