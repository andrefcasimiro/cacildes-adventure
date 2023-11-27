using UnityEngine;

namespace AF
{
    public class InflictStatusOnTriggerStay : MonoBehaviour
    {
        public StatusEffect statusEffect;
        public float amount = 10;
        public bool detectEnemies = true;

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject != null)
            {
                if (other.CompareTag("Player") && other.TryGetComponent(out PlayerManager playerManager))
                {
                    if (playerManager.playerStatsDatabase.currentHealth <= 0)
                    {
                        return;
                    }

                    playerManager.statusController.InflictStatusEffect(
                        statusEffect, amount * Time.deltaTime, false);
                }
                else if (detectEnemies && other.TryGetComponent(out CharacterManager characterManager))
                {
                    characterManager.statusController.InflictStatusEffect(
                        statusEffect, amount * Time.deltaTime, false);
                }
            }
        }

    }
}
