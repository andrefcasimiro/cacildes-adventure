using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class BallisticEngine : MonoBehaviour
    {
        public AudioSource audioSource;
        public AudioClip fireSfx;

        public Projectile prefab;

        public Transform[] spawnRefs;

        public float intervalBetweenShots = 2f;
        float currentInterval = Mathf.Infinity;

        private void Update()
        {
            if (currentInterval < intervalBetweenShots)
            {
                currentInterval += Time.deltaTime;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (currentInterval < intervalBetweenShots)
            {
                return;
            }

            if (other.gameObject.CompareTag("Player") == false)
            {
                return;
            }

            currentInterval = 0f;

            foreach (var spawnRef in spawnRefs)
            {
                var obj = Instantiate(prefab, spawnRef.transform.position, spawnRef.transform.rotation);
                obj.ShootForward();

                audioSource.PlayOneShot(fireSfx);
            }

        }

    }

}
