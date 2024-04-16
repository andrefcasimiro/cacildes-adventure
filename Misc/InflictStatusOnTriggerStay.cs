using System.Collections.Generic;
using System.Linq;
using AF.Health;
using UnityEngine;

namespace AF
{
    public class InflictStatusOnTriggerStay : MonoBehaviour
    {
        public bool detectEnemies = true;

        public Damage damage;

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject != null)
            {
                Damage newDamage = GetClonedDamage();

                if (other.CompareTag("Player") && other.TryGetComponent(out PlayerManager playerManager))
                {
                    if (playerManager.playerStatsDatabase.currentHealth <= 0)
                    {
                        return;
                    }

                    playerManager.damageReceiver.TakeDamage(newDamage);

                }
                else if (detectEnemies && other.TryGetComponent(out CharacterManager characterManager))
                {
                    characterManager.damageReceiver.TakeDamage(newDamage);
                }
            }
        }

        Damage GetClonedDamage()
        {
            List<StatusEffectEntry> statusEffectEntries = new();

            if (damage.statusEffects != null && damage.statusEffects.Length > 0)
            {
                foreach (var statusEffect in damage.statusEffects)
                {
                    // Create a deep copy of the StatusEffectEntry
                    StatusEffectEntry clonedStatusEffect = new StatusEffectEntry
                    {
                        statusEffect = statusEffect.statusEffect,
                        amountPerHit = statusEffect.amountPerHit
                    };
                    statusEffectEntries.Add(clonedStatusEffect);
                }
            }

            Damage newDamage = new Damage
            {
                statusEffects = statusEffectEntries.ToArray()
            };

            if (newDamage.statusEffects != null && newDamage.statusEffects.Length > 0)
            {
                for (int i = 0; i < newDamage.statusEffects.Length; i++)
                {
                    newDamage.statusEffects[i].amountPerHit = newDamage.statusEffects[i].amountPerHit * Time.deltaTime;
                }
            }

            return newDamage;
        }

    }
}
