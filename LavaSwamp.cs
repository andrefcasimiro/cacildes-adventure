using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class LavaSwamp : MonoBehaviour
    {
        [Header("Status Effect")]
        public float maxCooldownDamage = 1f;
        float cooldown = Mathf.Infinity;

        public float fireDamage = 50;

        public ParticleSystem fireFx;

        [Tooltip("If player dodges while in the poisonous swamp, how much damage is added")]
        public float dodgeBonusMultiplier = 2f;

        PlayerHealthbox playerHealthbox;

        private void Update()
        {
            if (cooldown < maxCooldownDamage) { cooldown += Time.deltaTime; }
        }

        private void OnTriggerStay(Collider other)
        {
            if (cooldown < maxCooldownDamage)
            {
                return;
            }

            if (!other.gameObject.CompareTag("Player"))
            {
                return;
            }

            var finalStatusAmount = fireDamage;
            if (other.GetComponent<DodgeController>().IsDodging())
            {
                finalStatusAmount *= dodgeBonusMultiplier;
            }

            if (playerHealthbox == null)
            {
                playerHealthbox = FindAnyObjectByType<PlayerHealthbox>(FindObjectsInactive.Include);
            }

            playerHealthbox.TakeEnvironmentalDamage(finalStatusAmount, 1, true, WeaponElementType.Fire);

            fireFx.transform.position = other.ClosestPoint(this.transform.position);
            fireFx.Play();
            fireFx.GetComponent<AudioSource>().Play();

            cooldown = 0f;
        }

    }

}
