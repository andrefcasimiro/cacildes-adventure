using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class EnemyPoiseController : MonoBehaviour
    {
        public readonly int hashTakingDamage = Animator.StringToHash("TakingDamage");

        [Header("Poise")]
        public int maxPoiseHits = 1;
        int currentPoiseHitCount = 0;
        float resetPositeTimer;
        public float maxTimeBeforeReset = 15f;
        
        public float maxCooldownBeforeTakingAnotherHitToPoise = 2f;
        float cooldownBeforeTakingAnotherHitToPoise = Mathf.Infinity;


        [Header("Sounds")]
        public AudioClip damageSfx;

        private Enemy enemy => GetComponent<Enemy>();
        private EnemyCombatController enemyCombatController => GetComponent<EnemyCombatController>();

        private void Update()
        {
            if (maxPoiseHits > 1)
            {
                resetPositeTimer += Time.deltaTime;

                if (resetPositeTimer >= maxTimeBeforeReset)
                {
                    currentPoiseHitCount = 0;
                    resetPositeTimer = 0f;
                }
            }

            if (cooldownBeforeTakingAnotherHitToPoise < maxCooldownBeforeTakingAnotherHitToPoise)
            {
                cooldownBeforeTakingAnotherHitToPoise += Time.deltaTime;
            }
        }

        public void IncreasePoiseDamage(int poiseDamage)
        {
            currentPoiseHitCount = Mathf.Clamp(currentPoiseHitCount + 1 + poiseDamage, 0, maxPoiseHits);


            if (currentPoiseHitCount >= maxPoiseHits)
            {
                ActivatePoiseDamage();
            }
            else
            {
                // If enemy was hit, force him into combat
                if (enemyCombatController.IsCombatting() == false || enemy.animator.GetBool(enemy.hashChasing) == false)
                {
                    enemy.agent.isStopped = true;
                    enemy.animator.SetBool(enemy.hashChasing, true);
                }
            }
        }

        public void ActivatePoiseDamage()
        {
            if (cooldownBeforeTakingAnotherHitToPoise < maxCooldownBeforeTakingAnotherHitToPoise)
            {
                return;
            }

            currentPoiseHitCount = 0;
    
            if (enemy.animator != null)
            {
                cooldownBeforeTakingAnotherHitToPoise = 0f;

                enemy.animator.SetTrigger(hashTakingDamage);

                BGMManager.instance.PlaySoundWithPitchVariation(damageSfx, enemyCombatController.combatAudioSource);
            }
        }


    }
}