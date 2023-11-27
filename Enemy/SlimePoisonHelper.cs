using System.Collections;
using System.Collections.Generic;
using AF.StatusEffects;
using UnityEngine;

namespace AF
{
    public class SlimePoisonHelper : MonoBehaviour
    {
        public float damagePerHit = 5f;
        public float statusEffectAmount;
        public StatusEffect statusEffect;

        //PlayerHealthbox playerHealthbox;
        StatusController statusController;

        private void Awake()
        {
            //  playerHealthbox = FindObjectOfType<PlayerHealthbox>(true);
            statusController = FindObjectOfType<StatusController>(true);
        }

        public void DamagePlayer()
        {
            //playerHealthbox.Event_TakeDamage(damagePerHit);

            if (statusEffect != null)
            {
                statusController.InflictStatusEffect(statusEffect, statusEffectAmount, false);
            }
        }

    }

}
