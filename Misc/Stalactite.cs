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

        float maxCooldownOnAir = 5f;
        float cooldownOnAir = Mathf.Infinity;

        private void Awake()
        {
            originalPosition = transform.localPosition;
        }

        private void Update()
        {
            if (cooldownOnAir < maxCooldownOnAir)
            {
                cooldownOnAir += Time.deltaTime;
            }

            if (cooldownOnAir >= maxCooldownOnAir)
            {
                ResetStalacites();
            }
        }

        public void Drop()
        {
            GetComponent<Rigidbody>().isKinematic = false;
            cooldownOnAir = 0f;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                FindObjectOfType<PlayerHealthbox>(true).TakeDamage(damageInflicted, this.transform, null, 5, WeaponElementType.None);
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
