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
                    playerStatusManager.InflictStatusEffect(statusEffect, amount * Time.deltaTime, false);
                }
                else if (detectEnemies)
                {
                    var enemy = other.GetComponent<EnemyManager>();

                    if (enemy != null && enemy.enemyNegativeStatusController != null)
                    {
                        enemy.enemyNegativeStatusController.InflictStatusEffect(statusEffect, amount * Time.deltaTime);
                    }
                }
            }
        }

    }
}
