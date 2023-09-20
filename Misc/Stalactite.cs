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

        PlayerHealthbox playerHealthbox;

        private void Awake()
        {
            playerHealthbox = FindObjectOfType<PlayerHealthbox>(true);

            originalPosition = transform.localPosition;

            this.gameObject.SetActive(false);
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

            GetComponent<MeshRenderer>().enabled = true;
            GetComponent<Rigidbody>().isKinematic = false;
            cooldownOnAir = 0f;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("EnemyHealthHitbox"))
            {
                return;
            }

            if (other.gameObject.CompareTag("Player"))
            {
                playerHealthbox.TakeDamage(damageInflicted, this.transform, null, 5, 0, WeaponElementType.None);
            }

            Instantiate(collisionWithGroundParticle, this.transform.position, Quaternion.identity);

            StartCoroutine(ResetStalacitesAfterAWhile());
        }

        IEnumerator ResetStalacitesAfterAWhile()
        {
            yield return new WaitForSeconds(3f);


            ResetStalacites();
        }

        private void ResetStalacites()
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<MeshRenderer>().enabled = false;

            transform.localPosition = originalPosition;

            this.gameObject.SetActive(false);
        }

    }

}
