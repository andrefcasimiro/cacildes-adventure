﻿using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.Events;

namespace AF
{
    public class EnemyManager : MonoBehaviour
    {
        #region Animation Hashes
        public readonly int hashIdle = Animator.StringToHash("Idle");
        public readonly int hashPatrol = Animator.StringToHash("Patrolling");
        public readonly int hashChasing = Animator.StringToHash("Chasing");

        public readonly int hashIsInCombat = Animator.StringToHash("IsInCombat");

        public readonly int hashCombatting = Animator.StringToHash("Combatting");
        public readonly int hashWaiting = Animator.StringToHash("Waiting");
        public readonly int hashIsWaiting = Animator.StringToHash("IsWaiting");

        public readonly int hashIsBlocking = Animator.StringToHash("IsBlocking");

        public readonly int hashIsBuffing = Animator.StringToHash("IsBuffing");

        public readonly int hashIsSleeping = Animator.StringToHash("IsSleeping");
        public readonly int hashSleeping = Animator.StringToHash("Sleeping");

        public readonly int hashIsFalling = Animator.StringToHash("IsFalling");
        public readonly int hashFalling = Animator.StringToHash("Falling");

        public readonly int hashPostureHit = Animator.StringToHash("PostureHit");
        public readonly int hashPostureBreak = Animator.StringToHash("PostureBreak");
        public readonly int hashIsStunned = Animator.StringToHash("IsStunned");

        public readonly int hashTurnBehind = Animator.StringToHash("TurnBehind");
        public readonly int hashTurnLeft = Animator.StringToHash("TurnLeft");
        public readonly int hashTurnRight = Animator.StringToHash("TurnRight");

        public readonly int hashShooting = Animator.StringToHash("Shooting");
        public readonly int hashIsShooting = Animator.StringToHash("IsShooting");

        public readonly int hashTakingDamage = Animator.StringToHash("TakingDamage");

        public readonly int hashIsStrafing = Animator.StringToHash("IsStrafing");

        public readonly int hashDying = Animator.StringToHash("Dying");
        public readonly int hashIsDead = Animator.StringToHash("Dead");

        public readonly int hashDodgeLeft = Animator.StringToHash("Dodge_Left");
        public readonly int hashDodgeRight = Animator.StringToHash("Dodge_Right");

        #endregion

        [Header("Components")]
        public Animator animator;
        public NavMeshAgent agent;
        public new Rigidbody rigidbody => GetComponent<Rigidbody>();

        [Header("Music")]
        public bool shouldPlayBattleMusic = true;

        [Header("Enemy")]
        public Enemy enemy;
        public int currentLevel;
        public int healthOverride = -1;

        [Header("Chasing")]
        public float maximumChaseDistance = 10f;
        public bool alwaysTrackPlayer = false;

        [Header("Gravity")]
        public bool canFall = true;

        [Header("Audio Sources")]
        public AudioSource combatAudioSource;

        // Flags
        [HideInInspector] public bool facePlayer = false;

        [Header("Rotation Options")]
        public bool rotateWithAnimations = true;
        public float rotationSpeed = 5f;
        [HideInInspector] public GameObject player;

        // Components
        [HideInInspector] public EnemyPatrolController enemyPatrolController => GetComponent<EnemyPatrolController>();
        [HideInInspector] public EnemySightController enemySightController => GetComponent<EnemySightController>();
        [HideInInspector] public EnemyLoot enemyLoot => GetComponent<EnemyLoot>();
        [HideInInspector] public EnemyBehaviorController enemyFactionController => GetComponent<EnemyBehaviorController>();
        [HideInInspector] public EnemyHealthController enemyHealthController => GetComponent<EnemyHealthController>();
        [HideInInspector] public EnemyBossController enemyBossController => GetComponent<EnemyBossController>();
        [HideInInspector] public EnemyBuffController enemyBuffController => GetComponent<EnemyBuffController>();
        [HideInInspector] public EnemyPoiseController enemyPoiseController => GetComponent<EnemyPoiseController>();
        [HideInInspector] public EnemyPostureController enemyPostureController => GetComponent<EnemyPostureController>();
        [HideInInspector] public EnemySleepController enemySleepController => GetComponent<EnemySleepController>();
        [HideInInspector] public EnemyCombatController enemyCombatController => GetComponent<EnemyCombatController>();
        [HideInInspector] public EnemyWeaponController enemyWeaponController => GetComponent<EnemyWeaponController>();
        [HideInInspector] public EnemyBlockController enemyBlockController => GetComponent<EnemyBlockController>();
        [HideInInspector] public EnemyProjectileController enemyProjectileController => GetComponent<EnemyProjectileController>();
        [HideInInspector] public EnemyDodgeController enemyDodgeController => GetComponent<EnemyDodgeController>();
        [HideInInspector] public EnemyTargetController enemyTargetController => GetComponent<EnemyTargetController>();
        [HideInInspector] public EnemyBehaviorController enemyBehaviorController => GetComponent<EnemyBehaviorController>();
        [HideInInspector] public EnemyNegativeStatusController enemyNegativeStatusController => GetComponent<EnemyNegativeStatusController>();

        [HideInInspector] public EventPage eventPage => GetComponent<EventPage>();

        Vector3 initialPosition; // For bonfire respawns

        bool usesGravityByDefault;
        bool isKinematicByDefault;

        private void Awake()
        {
            player = GameObject.FindWithTag("Player");
        }

        private void Start()
        {
            usesGravityByDefault = rigidbody.useGravity;
            isKinematicByDefault = rigidbody.isKinematic;

            initialPosition = transform.position;
        }

        private void Update()
        {
            if (facePlayer)
            {
                if (enemyTargetController.ignoreCompanions == false && enemyTargetController.currentCompanion != null)
                {
                    FaceTarget(enemyTargetController.currentCompanion.transform);
                }
                else
                {
                    FacePlayer();
                }
            }
        }

        public void FacePlayer()
        {
            FaceTarget(player.transform);
        }

        void FaceTarget(Transform target)
        {
            var lookRotation = target.position - this.transform.position;
            lookRotation.y = 0;
            var rotation = Quaternion.LookRotation(lookRotation);
            transform.rotation = Quaternion.Lerp(this.transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }

        public void Revive()
        {
            animator.SetBool(hashDying, false);

            enemyHealthController.currentHealth = enemyHealthController.GetMaxHealth();
            enemyHealthController.EnableHealthHitboxes();
            enemyHealthController.InitializeEnemyHUD();

            agent.SetDestination(initialPosition);
            this.transform.position = initialPosition;
            agent.nextPosition = initialPosition;

            var deathCollider = GetComponentInChildren<DeathColliderRef>(true);
            if (deathCollider != null)
            {
                deathCollider.GetComponent<BoxCollider>().enabled = false;
            }

            foreach (var colliderToDisableOnDeath in GetComponents<Collider>())
            {
                colliderToDisableOnDeath.enabled = true;
            }

            rigidbody.isKinematic = isKinematicByDefault;
            GetComponent<CapsuleCollider>().enabled = true;

            agent.enabled = true;

            animator.Play(hashIdle);

            // Remove all negative status
            enemyNegativeStatusController.ClearAllNegativeStatus();

            if (enemyPostureController != null)
            {
                enemyPostureController.currentPostureDamage = 0;
                enemyPostureController.InitializePostureHUD();
            }

            // Clear damage text
            var damageTexts = GetComponentsInChildren<FloatingText>(true);
            foreach (var damageText in damageTexts)
            {
                damageText.gameObject.SetActive(true);
                damageText.Reset();
            }
        }

        public bool PlayerIsBehind()
        {
            Vector3 targetDirection = player.transform.position - transform.position;
            float viewableAngle = Vector3.SignedAngle(targetDirection, transform.forward, Vector3.up);

            if (viewableAngle >= 100 && viewableAngle <= 180)
            {
                return true;
            }

            if (viewableAngle <= -101 && viewableAngle >= -180)
            {
                return true;
            }

            return false;
        }

        public bool PlayerIsOnTheLeft()
        {
            Vector3 targetDirection = player.transform.position - transform.position;
            float viewableAngle = Vector3.SignedAngle(targetDirection, transform.forward, Vector3.up);

            if (viewableAngle >= 0 && viewableAngle <= 100)
            {
                return true;
            }

            return false;
        }

        public bool PlayerIsOnTheRight()
        {
            Vector3 targetDirection = player.transform.position - transform.position;
            float viewableAngle = Vector3.SignedAngle(targetDirection, transform.forward, Vector3.up);

            if (viewableAngle < 0 && viewableAngle >= -100)
            {
                return true;
            }

            return false;
        }

        public bool IsGrounded()
        {

            RaycastHit hit;
            Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), animator.transform.up * -1f, out hit, 2f);
            // Debug.DrawRay(transform.position + new Vector3(0, 0.1f, 0), animator.transform.up * -1f, Color.red);


            return hit.transform != null;
        }

        public bool IsNavMeshAgentActive()
        {
            return agent.enabled && agent.isOnNavMesh;
        }

        public void ReenableNavmesh()
        {
            NavMeshHit rightHit;
            NavMesh.SamplePosition(transform.position, out rightHit, 999f, NavMesh.AllAreas);

            agent.enabled = false;
            agent.nextPosition = rightHit.position;

            agent.updatePosition = true;
            agent.enabled = true;

            rigidbody.useGravity = usesGravityByDefault;
            rigidbody.isKinematic = isKinematicByDefault;
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;

            if (animator.GetBool(hashIsWaiting) == false)
            {
                animator.CrossFade( hashWaiting, 0.05f);
            }
        }
    }
}