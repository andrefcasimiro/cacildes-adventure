using System.Collections;
using System.Collections.Generic;
using AF.Health;
using UnityEngine;

namespace AF
{
    public class Stalactite : MonoBehaviour
    {
        public DamageReceiver damageOwner;
        public Damage damage;
        public GameObject collisionWithGroundParticle;

        Vector3 originalPosition;

        float maxCooldownOnAir = 5f;
        float cooldownOnAir = Mathf.Infinity;

        private void Awake()
        {
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
            other.TryGetComponent<DamageReceiver>(out var damageReceiver);
            if (damageReceiver == null || damageReceiver == damageOwner)
            {
                return;
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
