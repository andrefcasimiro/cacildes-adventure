using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class ArenaPowerup : MonoBehaviour
    {
        public Consumable consumable;

        public float maxTime = 10f;
        float currentTime = 0f;

        public AudioClip onCollisionWithPlayer;

        [Header("Databases")]
        public StatusDatabase statusDatabase;

        private void Update()
        {
            currentTime += Time.deltaTime;

            if (currentTime >= maxTime)
            {
                Destroy(this.gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                BGMManager.instance.PlaySound(onCollisionWithPlayer, null);

                consumable.StartEffectOnPlayer(statusDatabase);

                Destroy(this.gameObject);
            }
        }

    }

}