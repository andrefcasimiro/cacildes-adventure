using System.Collections;
using UnityEngine;

namespace AF
{
    public class PlayerPoiseController : MonoBehaviour
    {
        public readonly int hashStunnedDamage = Animator.StringToHash("StunnedDamage");
        public readonly int hashTakingDamage = Animator.StringToHash("TakingDamage");

        [Header("Posture")]
        public int maxPoiseCountBeforeStunned = 5;
        public int maxPoiseHits = 1;
        int currentPoiseHitCount = 0;
        float timerBeforeReset;
        public float maxTimeBeforeReset = 15f;

        [Header("Sounds")]
        public AudioClip damageSfx;

        PlayerCombatController playerCombatController => GetComponent<PlayerCombatController>();
        PlayerComponentManager playerComponentManager => GetComponent<PlayerComponentManager>();

        ClimbController climbController => GetComponent<ClimbController>();

        Animator animator => GetComponent<Animator>();

        [Header("Cooldowns Before Enabling Components")]
        public float hitDisabledTime = .65f;
        public float stunnedDisableTime = 2.5f;

        [Header("Visuals")]
        public DestroyableParticle damageParticlePrefab;

        public bool isStunned = false;

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
            currentPoiseHitCount = currentPoiseHitCount + 1 + poiseDamage;

            if (climbController.climbState != ClimbController.ClimbState.NONE)
            {
                return;
            }

            if (currentPoiseHitCount >= maxPoiseHits)
            {
                ActivatePoiseDamage();
            }
        }

        public void ActivatePoiseDamage()
        {
            if (damageParticlePrefab != null)
            {
                Instantiate(damageParticlePrefab, transform.position, Quaternion.identity);
            }

            if (currentPoiseHitCount >= maxPoiseCountBeforeStunned)
            {
                isStunned = true;
                animator.Play(hashStunnedDamage);
                StartCoroutine(RecoverControl(stunnedDisableTime));
            }
            else
            {
                animator.Play(hashTakingDamage);
                StartCoroutine(RecoverControl(hitDisabledTime));
            }

            StartCoroutine(PlayHurtSfx());

            currentPoiseHitCount = 0;
            playerComponentManager.DisableComponents();
            playerComponentManager.DisableCharacterController();
        }

        IEnumerator RecoverControl(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            playerComponentManager.EnableCharacterController();
            playerComponentManager.EnableComponents();
            isStunned = false;
        }

        public IEnumerator PlayHurtSfx()
        {
            yield return new WaitForSeconds(0.1f);

            BGMManager.instance.PlaySoundWithPitchVariation(damageSfx, playerCombatController.combatAudioSource);
        }
    }
}