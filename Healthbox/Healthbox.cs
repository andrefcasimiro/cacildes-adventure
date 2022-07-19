using System.Collections;
using UnityEngine;

namespace AF
{
    [RequireComponent(typeof(Animator))]
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
        protected Animator animator => GetComponent<Animator>();
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

        public virtual void TakeDamage(float damage, Transform attackerTransform, string attackerName, AudioClip attackerWeaponSwingSfx)
        {
        }

        protected IEnumerator PlayHurtSfx()
        {
            yield return new WaitForSeconds(0.1f);
            Utils.PlaySfx(combatAudioSource, damageSfx);
        }

        public void Die()
        {
            animator.SetFloat(character.hashMovementSpeed, 0f);
            animator.SetTrigger(hashDying);
        }
    }
}
