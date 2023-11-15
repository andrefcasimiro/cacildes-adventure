using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class InflictStatusOnTriggerStay : MonoBehaviour
    {
        public StatusEffect statusEffect;
        public float amount = 10;

        public bool detectEnemies = true;

        PlayerStatusManager playerStatusManager;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;

        private void Awake()
        {
            playerStatusManager = FindObjectOfType<PlayerStatusManager>(true);
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject != null)
            {
                if (other.CompareTag("Player"))
                {
                    if (playerStatsDatabase.currentHealth <= 0)
                    {
                        return;
                    }

                    playerStatusManager.InflictStatusEffect(statusEffect, amount * Time.deltaTime, false);
                }
                else if (detectEnemies)
                {
                    var enemy = other.GetComponent<CharacterManager>();

                    if (enemy != null) //&& enemy.enemyNegativeStatusController != null)
                    {
                        if (false) //enemy.enemyHealthController.currentHealth <= 0)
                        {
                            return;
                        }

                        // enemy.enemyNegativeStatusController.InflictStatusEffect(statusEffect, amount * Time.deltaTime);
                    }
                }
            }
        }

    }
}
