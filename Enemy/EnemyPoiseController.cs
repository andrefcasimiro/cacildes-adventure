using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class EnemyPoiseController : MonoBehaviour
    {
        int currentPoiseHitCount = 0;
        float resetPoiseTimer;
        float cooldownBeforeTakingAnotherHitToPoise = Mathf.Infinity;

        EnemyManager enemyManager => GetComponent<EnemyManager>();

        void Update()
        {
            UpdatePoise();
        }


        void UpdatePoise()
        {
            if (enemyManager.enemy.maxPoiseHits > 1)
            {
                resetPoiseTimer += Time.deltaTime;

                if (resetPoiseTimer >= enemyManager.enemy.maxTimeBeforeResettingPoise)
                {
                    currentPoiseHitCount = 0;
                    resetPoiseTimer = 0f;
                }
            }

            if (cooldownBeforeTakingAnotherHitToPoise < enemyManager.enemy.maxCooldownBeforeTakingAnotherHitToPoise)
            {
                cooldownBeforeTakingAnotherHitToPoise += Time.deltaTime;
            }
        }

        public void IncreasePoiseDamage(int poiseDamage)
        {
            currentPoiseHitCount = Mathf.Clamp(currentPoiseHitCount + 1 + poiseDamage, 0, enemyManager.enemy.maxPoiseHits);

            if (currentPoiseHitCount >= enemyManager.enemy.maxPoiseHits)
            {
                ActivatePoiseDamage();
            }
            else
            {
                // If enemy was hit, force him into combat
                if (enemyManager.enemyCombatController.IsInCombat() == false)
                {
                    enemyManager.animator.SetBool(enemyManager.hashChasing, true);
                }
            }
        }


        public void ActivatePoiseDamage()
        {
            if (cooldownBeforeTakingAnotherHitToPoise < enemyManager.enemy.maxCooldownBeforeTakingAnotherHitToPoise)
            {
                return;
            }

            currentPoiseHitCount = 0;

            if (enemyManager.animator != null)
            {

                cooldownBeforeTakingAnotherHitToPoise = 0f;

                if (enemyManager.enemyCombatController.hasHyperArmorActive == false)
                {
                    enemyManager.animator.SetTrigger(enemyManager.hashTakingDamage);
                }

                // Should be tied to animation perhaps
                if (enemyManager.enemyBuffController != null)
                {
                    enemyManager.enemyBuffController.InterruptAllBuffs();
                }

                BGMManager.instance.PlaySoundWithPitchVariation(enemyManager.enemy.isMale
                    ? enemyManager.enemy.hurtSfx : enemyManager.enemy.femaleHurtSfx,
                    enemyManager.combatAudioSource);
            }
        }
    }

}
