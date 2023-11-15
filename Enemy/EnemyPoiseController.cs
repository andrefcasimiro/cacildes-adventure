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

        CharacterManager characterManager => GetComponent<CharacterManager>();

        void Update()
        {
            UpdatePoise();
        }


        void UpdatePoise()
        {
            /* if (characterManager.enemy.maxPoiseHits > 1)
             {
                 resetPoiseTimer += Time.deltaTime;

                 if (resetPoiseTimer >= characterManager.enemy.maxTimeBeforeResettingPoise)
                 {
                     currentPoiseHitCount = 0;
                     resetPoiseTimer = 0f;
                 }
             }

             if (cooldownBeforeTakingAnotherHitToPoise < characterManager.enemy.maxCooldownBeforeTakingAnotherHitToPoise)
             {
                 cooldownBeforeTakingAnotherHitToPoise += Time.deltaTime;
             }*/
        }

        public void IncreasePoiseDamage(int poiseDamage)
        {
            //currentPoiseHitCount = Mathf.Clamp(currentPoiseHitCount + 1 + poiseDamage, 0, characterManager.enemy.maxPoiseHits);

            /*if (currentPoiseHitCount >= characterManager.enemy.maxPoiseHits)
            {
                ActivatePoiseDamage();
            }
            else
            {
                // If enemy was hit, force him into combat
                if (characterManager.enemyCombatController.IsInCombat() == false)
                {
                    // characterManager.animator.SetBool(characterManager.hashChasing, true);
                }
            }*/
        }


        public void ActivatePoiseDamage()
        {
            if (false)//cooldownBeforeTakingAnotherHitToPoise < characterManager.enemy.maxCooldownBeforeTakingAnotherHitToPoise)
            {
                return;
            }

            currentPoiseHitCount = 0;

            if (characterManager.animator != null)
            {

                cooldownBeforeTakingAnotherHitToPoise = 0f;

                /*if (characterManager.enemyCombatController.hasHyperArmorActive == false)
                {
                    // characterManager.animator.SetTrigger(characterManager.hashTakingDamage);
                }


                BGMManager.instance.PlaySoundWithPitchVariation(characterManager.enemy.isMale
                    ? characterManager.enemy.hurtSfx : characterManager.enemy.femaleHurtSfx,
                    characterManager.combatAudioSource);*/
            }
        }
    }

}
