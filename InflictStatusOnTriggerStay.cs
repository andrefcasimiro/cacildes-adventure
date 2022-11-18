using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class InflictStatusOnTriggerStay : MonoBehaviour
    {
        public StatusEffect statusEffect;
        public float amount = 0.1f;

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject != null)
            {
                var playerStatus = other.GetComponent<PlayerStatusManager>();

                if (playerStatus != null)
                {
                    playerStatus.InflictStatusEffect(statusEffect, amount * Time.deltaTime, false);
                }
                else
                {
                    var enemyStatus = other.GetComponent<EnemyStatusManager>();

                    if (enemyStatus != null)
                    {
                        enemyStatus.InflictStatusEffect(statusEffect, amount * Time.deltaTime);
                    }
                }


            }
        }

    }

}
