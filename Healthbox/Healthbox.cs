using System.Collections;
using UnityEngine;

namespace AF
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class Healthbox : MonoBehaviour
    {
        public readonly int hashTakingDamage = Animator.StringToHash("TakingDamage");
        public readonly int hashDying = Animator.StringToHash("Dying");
        public readonly int hashIsDead = Animator.StringToHash("Dead");

        [Header("Sounds")]
        public AudioSource combatAudioSource;
        public AudioClip damageSfx;
        public AudioClip deathGruntSfx;

        protected Character character => GetComponent<Character>();
        public Animator animator;
        protected ICombatable combatable => GetComponent<ICombatable>();

        public DestroyableParticle damageParticlePrefab;

        public void SetHealth(float amount)
        {
            combatable.SetCurrentHealth(amount);

            if (combatable.GetCurrentHealth() > 0)
            {
                animator.SetBool(hashIsDead, false);
            }
        }

        public virtual void TakeDamage(float damage, Transform attackerTransform,AudioClip attackerWeaponSwingSfx)
        {
        }

        protected IEnumerator PlayHurtSfx()
        {
            yield return new WaitForSeconds(0.1f);
            Utils.PlaySfx(combatAudioSource, damageSfx);
        }

        protected IEnumerator PlayDeathSfx()
        {
            yield return new WaitForSeconds(0.1f);
            Utils.PlaySfx(combatAudioSource, deathGruntSfx);
        }

        public void Die()
        {
            StartCoroutine(PlayDeathSfx());

            animator.SetFloat(character.hashMovementSpeed, 0f);
            animator.SetTrigger(hashDying);
        }

        public void DieWithSound()
        {
            StartCoroutine(PlayDeathSfx());
            animator.SetFloat(character.hashMovementSpeed, 0f);
            animator.SetTrigger(hashDying);
        }
    }
}
