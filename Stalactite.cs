using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class Stalactite : MonoBehaviour
    {
        public float damageInflicted;
        public GameObject collisionWithGroundParticle;

        Vector3 originalPosition;

        private void Awake()
        {
            originalPosition = transform.localPosition;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                other.GetComponent<PlayerHealthbox>().TakeDamage(damageInflicted, this.transform, null);
                Instantiate(collisionWithGroundParticle, this.transform.position, Quaternion.identity);
                ResetStalacites();
            }

            if (other.gameObject.tag == "Mud")
            {
                Instantiate(collisionWithGroundParticle, this.transform.position, Quaternion.identity);

                ResetStalacites();
            }
        }

        private void ResetStalacites()
        {
            GetComponent<Rigidbody>().isKinematic = true;

            transform.localPosition = originalPosition;
        }

    }

}
