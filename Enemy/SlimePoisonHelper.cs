using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class SlimePoisonHelper : MonoBehaviour
    {
        public float damagePerHit = 5f;
        public float statusEffectAmount;
        public StatusEffect statusEffect;

        PlayerHealthbox playerHealthbox;
        PlayerStatusManager playerStatusManager;

        private void Awake()
        {
            playerHealthbox = FindObjectOfType<PlayerHealthbox>(true);
            playerStatusManager = FindObjectOfType<PlayerStatusManager>(true);
        }

        public void DamagePlayer()
        {
            playerHealthbox.Event_TakeDamage(damagePerHit);

            if (statusEffect != null)
            {
                playerStatusManager.InflictStatusEffect(statusEffect, statusEffectAmount, false);
            }
        }

    }

}
