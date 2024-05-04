using System.Collections;
using System.Linq;
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

        [Header("Faction")]
        public CharacterFaction[] characterFactions;

        [Header("Flags")]
        public bool isBusy = false;

        public bool isConfused = false;

        [Header("Components")]
        public StatusController statusController;
        public CharacterBaseHealth health;
        public CharacterAbstractPosture characterPosture;
        public CharacterAbstractPoise characterPoise;
        public CharacterAbstractBlockController characterBlockController;
        public DamageReceiver damageReceiver;
        public CharacterPushController characterPushController;
        public abstract void ResetStates();

        public bool IsBusy()
        {
            return isBusy;
        }

        public void SetIsBusy(bool value)
        {
            isBusy = value;
        }

        public void PlayAnimationWithCrossFade(string animationName)
        {
            PlayAnimationWithCrossFade(animationName, false, false, 0.2f);
        }

        public void PlayAnimationWithCrossFade(string animationName, bool isBusy, bool applyRootMotion, float crossFade)
        {
            this.isBusy = isBusy;
            animator.applyRootMotion = applyRootMotion;

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


        public void PlayCrossFadeBusyAnimationWithRootMotion(string animationName, float crossFade)
        {
            animator.applyRootMotion = true;
            isBusy = true;
            animator.CrossFade(animationName, crossFade);
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

        public bool IsFromSameFaction(CharacterBaseManager target)
        {
            return target != null && characterFactions != null
                && characterFactions.Length > 0
                && characterFactions.Any(thisCharactersFaction =>
                    target.characterFactions != null && target.characterFactions.Length > 0 && target.characterFactions.Contains(thisCharactersFaction));

        }


        public void SetIsConfused(bool value)
        {
            this.isConfused = value;
        }

        public void ResetIsConfused()
        {
            this.isConfused = false;
        }
    }
}
