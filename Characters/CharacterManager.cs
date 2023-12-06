using AF.Animations;
using AF.Combat;
using AF.Equipment;
using AF.Health;
using AF.Shooting;
using UnityEngine;

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

        private void Awake()
        {
            animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);

            animator.runtimeAnimatorController = animatorOverrideController;
        }

        public override void ResetStates()
        {
            agent.enabled = true;
            animator.applyRootMotion = false;
            isBusy = false;

            characterPosture.ResetStates();
            characterCombatController.ResetStates();
            characterWeaponsManager.ResetStates();
            targetManager.ResetStates();
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
            if (animator.applyRootMotion)
            {
                agent.enabled = false;

                // Extract root motion position and rotation from the Animator
                Vector3 rootMotionPosition = animator.deltaPosition + new Vector3(0.0f, -9, 0.0f) * Time.deltaTime;

                Quaternion rootMotionRotation = animator.deltaRotation;

                // Apply root motion to the NavMesh Agent
                characterController.Move(rootMotionPosition);
                transform.rotation *= rootMotionRotation;
            }
        }

        public override Damage GetAttackDamage()
        {
            return characterCombatController.currentCombatAction?.damage;
        }
    }
}
