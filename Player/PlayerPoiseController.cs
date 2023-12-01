using System.Collections;
using AF.Ladders;
using AF.Stats;
using UnityEngine;

namespace AF
{
    public class PlayerPoiseController : MonoBehaviour
    {
        public readonly int hashStunnedDamage = Animator.StringToHash("StunnedDamage");
        public readonly int hashTakingDamage = Animator.StringToHash("TakingDamage");
        public readonly int hashIsTakingDamage = Animator.StringToHash("IsTakingDamage");
        public readonly int hashIsStunned = Animator.StringToHash("IsStunned");
        public readonly int hashParried = Animator.StringToHash("Parried");

        [Header("Components")]
        public PlayerManager playerManager;
        public LockOnManager lockOnManager;

        [Header("Posture")]
        public int unarmedPoiseHits = 1;
        public int poiseBonus = 0;
        int currentPoiseHitCount = 0;
        float timerBeforeReset;
        public float maxTimeBeforeReset = 15f;

        [Header("Sounds")]
        public AudioClip damageSfx;

        Animator animator => GetComponent<Animator>();

        [Header("Cooldowns Before Enabling Components")]
        public float hitDisabledTime = .65f;
        public float stunnedDisableTime = 2.5f;

        [Header("Visuals")]
        public DestroyableParticle damageParticlePrefab;

        public bool isStunned = false;

        float characterSlopeLimit, characterStepOffset;


        private void Awake()
        {
        }

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

            if (playerManager.climbController.climbState != ClimbState.NONE)
            {
                return;
            }

            var playerMaxPoise = GetMaxPoise();
            var poiseDamageReceived = Mathf.Abs(poiseDamage - playerMaxPoise);
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
                MarkAsStunned();
                animator.Play(hashStunnedDamage);
                lockOnManager.DisableLockOn();
            }
            else
            {
                animator.Play(hashTakingDamage);
                StartCoroutine(RecoverControl(hitDisabledTime));
            }

            StartCoroutine(PlayHurtSfx());

            currentPoiseHitCount = 0;
            playerManager.playerComponentManager.DisableComponents();
        }

        public void MarkAsStunned()
        {
            StartCoroutine(RecoverControl(stunnedDisableTime));
        }

        IEnumerator RecoverControl(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            playerManager.playerComponentManager.EnableCharacterController();
            playerManager.playerComponentManager.EnableComponents();

            yield return new WaitForSeconds(0.25f);
            isStunned = false;
        }

        public IEnumerator PlayHurtSfx()
        {
            yield return new WaitForSeconds(0.1f);

            BGMManager.instance.PlaySoundWithPitchVariation(damageSfx, playerManager.playerCombatController.combatAudioSource);
        }

        public int GetMaxPoise()
        {
            return unarmedPoiseHits + playerManager.statsBonusController.equipmentPoise + poiseBonus;
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
