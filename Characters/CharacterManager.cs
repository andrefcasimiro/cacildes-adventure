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
using AF.Companions;
using UnityEngine.AI;
using System;

namespace AF
{
    public class CharacterManager : CharacterBaseManager
    {
        public CompanionID companionID;
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

        [Header("Settings")]
        public bool canRevive = true;
        public bool shouldReturnToInitialPositionOnRevive = true;

        [Header("Face Target Settings")]
        public bool faceTarget = false;
        public float faceTargetDuration = 0.25f;
        public bool alwaysFaceTarget = false;

        [Header("Partners")]
        public CharacterManager[] partners;
        public int partnerOrder = 0;

        [Header("Events")]
        public UnityEvent onResetStates;
        public UnityEvent onForceAgressionTowardsPlayer;

        // Scene Reference
        PlayerManager playerManager;

        int defaultAnimationHash;

        public GameSession gameSession;


        private void Awake()
        {
            animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);

            animator.runtimeAnimatorController = animatorOverrideController;

            initialPosition = transform.position;
            initialRotation = transform.rotation;

            EventManager.StartListening(EventMessages.ON_LEAVING_BONFIRE, Revive);
        }

        private void Start()
        {
            defaultAnimationHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
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
            if ((faceTarget || alwaysFaceTarget) && targetManager?.currentTarget != null)
            {
                var lookPos = targetManager.currentTarget.transform.position - transform.position;
                lookPos.y = 0;
                var lookRotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }


            if (animator.applyRootMotion)
            {
                // Extract root motion position and rotation from the Animator
                Vector3 rootMotionPosition = animator.deltaPosition + new Vector3(0.0f, -9, 0.0f) * Time.deltaTime;

                Quaternion rootMotionRotation = animator.deltaRotation;

                // Apply root motion to the NavMesh Agent
                if (characterController.enabled)
                {
                    characterController.Move(rootMotionPosition);
                }

                agent.Warp(characterController.transform.position);

                transform.rotation *= rootMotionRotation;
            }
        }

        public override Damage GetAttackDamage()
        {
            return characterCombatController?.currentCombatAction?.damage;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void FaceTarget()
        {
            faceTarget = true;
            Invoke(nameof(ResetFaceTargetFlag), faceTargetDuration);
        }

        public void ResetFaceTargetFlag()
        {
            faceTarget = false;
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
            if (characterBossController.IsBoss() || !canRevive)
            {
                return;
            }

            agent.speed = 0f;

            if (health is CharacterHealth characterHealth)
            {
                characterHealth.Revive();

                if (IsCompanion() == false)
                {
                    if (shouldReturnToInitialPositionOnRevive)
                    {
                        agent.Warp(initialPosition);
                        characterController.enabled = false;
                        transform.SetPositionAndRotation(initialPosition, initialRotation);
                        characterController.enabled = true;
                    }
                }

                ResetStates();

                if (defaultAnimationHash != -1)
                {
                    animator.Play(defaultAnimationHash);
                }

            }
        }

        public string GetCharacterID()
        {
            return companionID.GetCompanionID();
        }

        public bool IsCompanion()
        {
            return companionID != null;
        }

        public void TeleportNearPlayer()
        {
            Vector3 desiredPosition = GetPlayerManager().transform.position + (GetPlayerManager().transform.forward * -4.5f);
            NavMesh.SamplePosition(desiredPosition, out NavMeshHit hit, 15f, NavMesh.AllAreas);

            if (IsValidPosition(hit.position))
            {
                characterController.enabled = false;
                agent.enabled = false;
                transform.position = hit.position;
                agent.nextPosition = hit.position;
                agent.enabled = true;
                characterController.enabled = true;
            }
        }


        private bool IsValidPosition(Vector3 position)
        {
            // Check for Infinity or NaN values
            return !float.IsInfinity(position.x) && !float.IsInfinity(position.y) && !float.IsInfinity(position.z) &&
                   !float.IsNaN(position.x) && !float.IsNaN(position.y) && !float.IsNaN(position.z);
        }
    }
}
