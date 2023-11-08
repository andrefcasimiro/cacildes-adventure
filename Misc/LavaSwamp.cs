using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AF
{
    public class LavaSwamp : MonoBehaviour
    {
        [Header("Enemies to ignore")]
        public Enemy[] enemiesToIgnore;

        [Header("Status Effect")]
        public float maxCooldownDamage = 1f;
        float cooldown = Mathf.Infinity;

        public float baseDamage = 0;
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

            if (other.gameObject.CompareTag("Enemy"))
            {
                EnemyManager enemyManager = other.GetComponent<EnemyManager>();

                if (enemyManager != null)
                {
                    if (enemiesToIgnore.Contains(enemyManager.enemy))
                    {
                        return;
                    }

                    var enemyFireDamage = enemyManager.enemy.fireDamageBonus * fireDamage;
                    enemyManager.enemyHealthController.TakeEnvironmentalDamage(enemyFireDamage);
                }

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

            // Apply elemental defense reduction based on weaponElementType
            float elementalDefense = Mathf.Clamp(other.GetComponent<DefenseStatManager>().GetFireDefense() / 100, 0f, 1f);

            finalStatusAmount = (int)(finalStatusAmount * Mathf.Abs((1 - elementalDefense))); // Subtract elemental defense as a percentage


            if (playerHealthbox == null)
            {
                playerHealthbox = FindAnyObjectByType<PlayerHealthbox>(FindObjectsInactive.Include);
            }

            playerHealthbox.TakeEnvironmentalDamage(baseDamage, playerHealthbox.isImmuneToFireDamage ? 0 : 1, true, (int)finalStatusAmount, WeaponElementType.Fire);

            fireFx.transform.position = other.ClosestPoint(this.transform.position);
            fireFx.Play();
            fireFx.GetComponent<AudioSource>().Play();

            cooldown = 0f;
        }

    }

}
