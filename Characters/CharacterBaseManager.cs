using System.Collections;
using AF.Characters;
using AF.Combat;
using AF.Health;
using AF.StatusEffects;
using UnityEngine;
using UnityEngine.AI;

namespace AF
{
    public abstract class CharacterBaseManager : MonoBehaviour
    {

        [Header("Components")]
        public Animator animator;
        public NavMeshAgent agent;
        public CharacterController characterController;

        [Header("Audio Sources")]
        public AudioSource combatAudioSource;

        [Header("Components")]
        public StatusController statusController;
        public CharacterBaseHealth health;
        public CharacterPosture characterPosture;
        public CharacterPoise characterPoise;
        public CharacterBlockController characterBlockController;
        public DamageReceiver damageReceiver;

        [Header("Faction")]
        public CharacterFaction characterFaction;

        [Header("Flags")]
        public bool isBusy = false;
        public bool isPushed = false;

        public abstract void ResetStates();

        public bool IsBusy()
        {
            return isBusy;
        }

        public void PlayAnimationWithCrossFade(string animationName)
        {
            animator.CrossFade(animationName, 0.2f);
        }

        public void PlayBusyAnimation(string animationName)
        {
            isBusy = true;
            animator.Play(animationName);
        }

        public void PlayBusyAnimationWithRootMotion(string animationName)
        {
            animator.applyRootMotion = true;
            PlayBusyAnimation(animationName);
        }

        #region Hashed Animations
        public void PlayBusyHashedAnimationWithRootMotion(int hashedAnimationName)
        {
            animator.applyRootMotion = true;
            PlayBusyHashedAnimation(hashedAnimationName);
        }

        public void PlayBusyHashedAnimation(int animationName)
        {
            isBusy = true;
            animator.Play(animationName);
        }
        #endregion

        public abstract Damage GetAttackDamage();

        // Call this method when the character gets hit by the force
        public void ApplyForceSmoothly(Vector3 forceDirection, float pushForce, float duration)
        {
            if (!isPushed)
            {
                StartCoroutine(ApplyForceCoroutine(forceDirection, pushForce, duration));
            }
        }

        private IEnumerator ApplyForceCoroutine(Vector3 forceDirection, float pushForce, float duration)
        {
            float elapsed = 0f;
            isPushed = true;

            while (elapsed < duration)
            {
                float forceMagnitude = Mathf.Lerp(pushForce, 0f, elapsed / duration);
                characterController.Move(forceDirection * forceMagnitude * Time.deltaTime);
                elapsed += Time.deltaTime;
                yield return null;
            }

            isPushed = false;
        }
    }
}
