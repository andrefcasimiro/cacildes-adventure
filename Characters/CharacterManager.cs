﻿using AF.Animations;
using AF.Combat;
using AF.Equipment;
using AF.Health;
using AF.Shooting;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class CharacterManager : CharacterBaseManager
    {

        public CharacterCombatController characterCombatController;
        public TargetManager targetManager;

        public CharacterBaseShooter characterBaseShooter;
        public CharacterWeaponsManager characterWeaponsManager;

        // Animator Overrides
        [HideInInspector] public AnimatorOverrideController animatorOverrideController;

        Quaternion initialRotation;

        [Header("Settings")]
        public float patrolSpeed = 2f;
        public float chaseSpeed = 4.5f;

        [Header("Events")]
        public UnityEvent onResetStates;

        private void Awake()
        {
            animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);

            animator.runtimeAnimatorController = animatorOverrideController;

            initialRotation = transform.rotation;
        }

        public override void ResetStates()
        {
            animator.applyRootMotion = false;

            isBusy = false;
            characterPosture.ResetStates();
            characterCombatController.ResetStates();
            characterWeaponsManager.ResetStates();
            damageReceiver.ResetStates();
            onResetStates?.Invoke();
        }

        public void UpdateAnimatorOverrideControllerClips(string animationName, AnimationClip animationClip)
        {
            var clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
            animatorOverrideController.GetOverrides(clipOverrides);
            clipOverrides[animationName] = animationClip;
            animatorOverrideController.ApplyOverrides(clipOverrides);
        }

        private void OnAnimatorMove()
        {
            //agent.updatePosition = !animator.applyRootMotion;

            if (animator.applyRootMotion)
            {
                // Extract root motion position and rotation from the Animator
                Vector3 rootMotionPosition = animator.deltaPosition + new Vector3(0.0f, -9, 0.0f) * Time.deltaTime;

                Quaternion rootMotionRotation = animator.deltaRotation;

                // Apply root motion to the NavMesh Agent
                characterController.Move(rootMotionPosition);
                transform.rotation *= rootMotionRotation;
                //agent.nextPosition = characterController.transform.position;
            }
        }

        public override Damage GetAttackDamage()
        {
            return characterCombatController.currentCombatAction?.damage;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void FaceTarget(Transform target)
        {
            if (target == null)
            {
                return;
            }

            var lookPos = target.transform.position - transform.position;
            lookPos.y = 0;
            transform.rotation = Quaternion.LookRotation(lookPos);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void FaceInitialRotation()
        {
            transform.rotation = initialRotation;
        }
    }
}
