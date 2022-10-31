using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class EnemyPoiseController : MonoBehaviour
    {
        public readonly int hashTakingDamage = Animator.StringToHash("TakingDamage");

        [Header("Posture")]
        public int maxPoiseHits = 1;
        int currentPoiseHitCount = 0;
        float timerBeforeReset;
        public float maxTimeBeforeReset = 15f;

        [Header("Sounds")]
        public AudioClip damageSfx;

        private Enemy enemy => GetComponent<Enemy>();
        private EnemyCombatController enemyCombatController => GetComponent<EnemyCombatController>();

        private void Update()
        {
            if (maxPoiseHits > 1)
            {
                timerBeforeReset += Time.deltaTime;

                if (timerBeforeReset >= maxTimeBeforeReset)
                {
                    currentPoiseHitCount = 0;
                    timerBeforeReset = 0f;
                }
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
            currentPoiseHitCount = 0;

            if (enemy.animator != null)
            {
                enemy.animator.SetTrigger(hashTakingDamage);
            }
        }


        public IEnumerator PlayHurtSfx()
        {
            yield return new WaitForSeconds(0.1f);

            BGMManager.instance.PlaySoundWithPitchVariation(damageSfx, enemyCombatController.combatAudioSource);
        }


    }
}