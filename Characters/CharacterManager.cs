using System.Collections;
using AF.Animations;
using AF.Combat;
using AF.Equipment;
using AF.Events;
using AF.Health;
using AF.Shooting;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class CharacterManager : CharacterBaseManager
    {
        public string characterID = "";
        public CharacterCombatController characterCombatController;
        public TargetManager targetManager;

        public CharacterBaseShooter characterBaseShooter;
        public CharacterWeaponsManager characterWeaponsManager;
        public CharacterBossController characterBossController;

        // Animator Overrides
        [HideInInspector] public AnimatorOverrideController animatorOverrideController;

        Vector3 initialPosition;
        Quaternion initialRotation;

        [Header("Settings")]
        public float patrolSpeed = 2f;
        public float chaseSpeed = 4.5f;
        public float rotationSpeed = 6f;

        [Header("Partners")]
        public CharacterManager[] partners;
        public int partnerOrder = 0;

        [Header("Events")]
        public UnityEvent onResetStates;

        // Scene Reference
        PlayerManager playerManager;

        private void Awake()
        {
            animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);

            animator.runtimeAnimatorController = animatorOverrideController;

            initialPosition = transform.position;
            initialRotation = transform.rotation;

            EventManager.StartListening(EventMessages.ON_LEAVING_BONFIRE, Revive);

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
        public void FaceTarget()
        {
            if (targetManager == null)
            {
                return;
            }

            var lookPos = targetManager.currentTarget.transform.position - transform.position;
            lookPos.y = 0;
            transform.rotation = Quaternion.LookRotation(lookPos);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void FacePlayer()
        {
            var lookPos = GetPlayerManager().transform.position - transform.position;
            lookPos.y = 0;
            transform.rotation = Quaternion.LookRotation(lookPos);
        }

        PlayerManager GetPlayerManager()
        {
            if (playerManager == null)
            {
                playerManager = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include);
            }

            return playerManager;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void FaceInitialRotation()
        {
            transform.rotation = initialRotation;
        }

        void Revive()
        {
            if (characterBossController.IsBoss())
            {
                return;
            }

            if (health is CharacterHealth characterHealth)
            {
                transform.position = initialPosition;
                transform.rotation = initialRotation;
                characterHealth.Revive();

            }
        }

        public string GetCharacterID()
        {
            return characterID;
        }
    }
}
