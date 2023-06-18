using StarterAssets;
using System.Collections;
using UnityEngine;

namespace AF
{
    public class PlayerPoiseController : MonoBehaviour
    {
        public readonly int hashStunnedDamage = Animator.StringToHash("StunnedDamage");
        public readonly int hashTakingDamage = Animator.StringToHash("TakingDamage");
        public readonly int hashIsTakingDamage = Animator.StringToHash("IsTakingDamage");
        public readonly int hashParried = Animator.StringToHash("Parried");

        [Header("Posture")]
        public int unarmedPoiseHits = 1;
        int currentPoiseHitCount = 0;
        float timerBeforeReset;
        public float maxTimeBeforeReset = 15f;

        [Header("Sounds")]
        public AudioClip damageSfx;

        PlayerCombatController playerCombatController => GetComponent<PlayerCombatController>();
        PlayerComponentManager playerComponentManager => GetComponent<PlayerComponentManager>();
        ThirdPersonController tps => GetComponent<ThirdPersonController>();
        EquipmentGraphicsHandler equipmentGraphicsHandler => GetComponent<EquipmentGraphicsHandler>();

        ClimbController climbController => GetComponent<ClimbController>();

        Animator animator => GetComponent<Animator>();

        [Header("Cooldowns Before Enabling Components")]
        public float hitDisabledTime = .65f;
        public float stunnedDisableTime = 2.5f;

        [Header("Visuals")]
        public DestroyableParticle damageParticlePrefab;

        public bool isStunned = false;

        PlayerSpellManager playerSpellManager => GetComponent<PlayerSpellManager>();

        private void Update()
        {
            if (currentPoiseHitCount <= 0)
            {
                return;
            }

            timerBeforeReset += Time.deltaTime;

            if (timerBeforeReset >= maxTimeBeforeReset)
            {
                currentPoiseHitCount = 0;
                timerBeforeReset = 0f;
            }
        }

        public void IncreasePoiseDamage(int poiseDamage)
        {
            currentPoiseHitCount += Mathf.Abs(poiseDamage) + 1;

            if (climbController.climbState != ClimbController.ClimbState.NONE)
            {
                return;
            }

            var playerMaxPoise = GetMaxPoise();
            var poiseDamageReceived = Mathf.Abs(poiseDamage - playerMaxPoise);

            if (currentPoiseHitCount >= playerMaxPoise && playerSpellManager.ShouldInterruptSpell(poiseDamageReceived))
            {
                playerSpellManager.CancelAnySpells();

                ActivatePoiseDamage();
            }
        }

        public void ActivateMaxPoiseDamage()
        {
            currentPoiseHitCount = 999;

            ActivatePoiseDamage();
        }

        public void ActivatePoiseDamage()
        {
            if (damageParticlePrefab != null)
            {
                Instantiate(damageParticlePrefab, transform.position, Quaternion.identity);
            }

            if (currentPoiseHitCount >= GetMaxPoise() * 3)
            {
                isStunned = true;
                animator.Play(hashStunnedDamage);
                StartCoroutine(RecoverControl(stunnedDisableTime));
                FindObjectOfType<LockOnManager>(true).DisableLockOn();
            }
            else
            {
                animator.Play(hashTakingDamage);
                StartCoroutine(RecoverControl(hitDisabledTime));
            }

            StartCoroutine(PlayHurtSfx());

            currentPoiseHitCount = 0;
            playerComponentManager.DisableComponents();

            if (!tps.skateRotation)
            {
                playerComponentManager.DisableCharacterController();
            }
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

        public int GetMaxPoise()
        {
            return unarmedPoiseHits + equipmentGraphicsHandler.equipmentPoise;
        }

        public bool IsTakingDamage()
        {
            return animator.GetBool(hashIsTakingDamage);
        }

        public void PlayParried()
        {
            animator.Play(hashParried);
        }

    }
}