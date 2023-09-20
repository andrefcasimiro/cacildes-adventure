using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.Events;
using Quaternion = UnityEngine.Quaternion;
using System.Collections;
using System.Drawing;
using UnityEngine.PlayerLoop;
using UnityEngine.Profiling;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

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

        public readonly int hashGuardBreak = Animator.StringToHash("Guard Break");
        public readonly int hashParry = Animator.StringToHash("Parry");

        public readonly int hashIsGrounded = Animator.StringToHash("IsGrounded");

        #endregion

        [Header("Components")]
        public Animator animator;
        public AnimatorOverrideController animatorOverrideController;
        public NavMeshAgent agent;
        [HideInInspector] public CharacterController characterController => GetComponent<CharacterController>();

        [Header("Enemy")]
        public Enemy enemy;
        public int currentLevel;
        public float animatorSpeed = 1f;

        [Header("Chasing")]
        public float maximumChaseDistance = 10f;
        public bool alwaysTrackPlayer = false;
        public bool canChase = true;
        public float chaseVelocityOverride = -1f;
        public float walkSpeedOverride = -1f;

        [Header("Gravity")]
        public bool canFall = true;
        public float distToGround = 0.1f;
        float lastGroundedPositionY;
        float minimumFallHeightToTakeDamage = 1f;
        float damageMultiplierPerMeter = 65f;

        [Header("Audio Sources")]
        public AudioSource combatAudioSource;

        // Flags
        [HideInInspector] public bool facePlayer = false;

        [Header("Rotation Options")]
        public float rotationSpeed = 5f;
        [HideInInspector] public GameObject player;

        [Header("Revive Options")]
        public UnityEvent onRevive;
        public Transform customReviveTransformRef;
        public string customReviveDefaultAnimation;
        public string customStartAnimation;

        [Header("Combat Partner")]
        public EnemyManager combatPartner;

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
        [HideInInspector] public EnemyTeleportManager enemyTeleportManager => GetComponent<EnemyTeleportManager>();
        [HideInInspector] public EnemyPushableOnDeath enemyPushableOnDeath => GetComponent<EnemyPushableOnDeath>();


        [HideInInspector] public EventPage eventPage => GetComponent<EventPage>();

        Vector3 initialPosition; // For bonfire respawns
        Quaternion initialRotation; // For bonfire respawns

        Vector3 smoothDesiredDirection;
        bool isMovingSmoothly = false;

        [Header("Pushable Options")]
        public bool canBePushed = true;

        float defaultAnimatorSpeed;

        [HideInInspector] public float attackReducingFactor = 1;
        [HideInInspector] public float healthReducingFactor = 1;

        private void Awake()
        {
            player = GameObject.FindWithTag("Player");

            lastGroundedPositionY = transform.position.y;

            if (animatorOverrideController != null)
            {
                animator.runtimeAnimatorController = animatorOverrideController;
            }

            animator.speed = animatorSpeed;
            defaultAnimatorSpeed = animator.speed;

            if (string.IsNullOrEmpty(customStartAnimation))
            {
                animator.Play(customStartAnimation);
            }
        }

        private void Start()
        {
            initialPosition = transform.position;
            initialRotation = transform.rotation;


            ResetDifficulty();

            if (canFall == false)
            {
                animator.SetBool("IsGrounded", true);
            }
        }

        public void ResetDifficulty()
        {
            // Don't change currently active enemies
            if (enemyCombatController.IsInCombat())
            {
                return;
            }

            if (GamePreferences.instance.gameDifficulty == GamePreferences.GameDifficulty.EASY)
            {
                var animSpeed = Mathf.Clamp(animatorSpeed - 0.1f, 0.75f, Mathf.Infinity);
                animator.speed = animSpeed;
                attackReducingFactor = 4;
                healthReducingFactor = 1.5f;
            }
            else if (GamePreferences.instance.gameDifficulty == GamePreferences.GameDifficulty.MEDIUM)
            {
                animator.speed = defaultAnimatorSpeed;
                attackReducingFactor = 2;
                healthReducingFactor = 1;
            }
            else
            {
                animator.speed = defaultAnimatorSpeed;
                attackReducingFactor = 1;
                healthReducingFactor = 1;
            }

            enemyHealthController.UpdateMaxHealth();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(transform.position + transform.up * -1f, .5f);
        }

        private void Update()
        {
            if (canFall)
            {
                if (!characterController.isGrounded)
                {
                    if (enemyPostureController == null || enemyPostureController.IsStunned() == false)
                    {
                        characterController.Move(new Vector3(0.0f, -4.5f, 0.0f) * Time.deltaTime);

                        animator.SetBool("IsGrounded", CheckEnemyGrounded());
                    }
                }
                else
                {
                    animator.SetBool("IsGrounded", true);
                }
            }

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

            if (isMovingSmoothly)
            {
                // Apply the force over time using lerp or move towards
                smoothDesiredDirection = Vector3.Lerp(smoothDesiredDirection, Vector3.zero, 0.1f); // Adjust the lerp factor (0.1f) for desired smoothing

                // Move the character controller using the force vector
                characterController.Move(smoothDesiredDirection * Time.deltaTime);

                // Check if the movement has reached the desired position or condition
                if (smoothDesiredDirection.magnitude < 0.01f) // Adjust the threshold value for completion
                {
                    smoothDesiredDirection = Vector3.zero;

                    isMovingSmoothly = false;
                }
            }
        }

        bool CheckEnemyGrounded()
        {
            // Cast a ray downward from the player's position
            Ray groundRay = new Ray(transform.position, Vector3.down);

            // Set the maximum distance the ray can travel
            float maxDistance = 2f;

            // Create a RaycastHit variable to store the information about the hit
            RaycastHit hit;

            // Perform the raycast and check if it hits something
            if (Physics.Raycast(groundRay, out hit, maxDistance))
            {
                // You can add more conditions here if needed, e.g., check for specific tags or layers.
                return true; // The enemy is grounded
            }

            return false; // The enemy is not grounded
        }

        public void CallCombatPartner()
        {
            if (combatPartner == null || combatPartner.enemyCombatController.IsWaiting() || combatPartner.enemyCombatController.IsInCombat())
            {
                return;
            }

            combatPartner.enemyCombatController.ForceIntoCombat();
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

        public void FacePlayerImmediately()
        {
            var lookRotation = player.transform.position - this.transform.position;
            lookRotation.y = 0;
            var rotation = Quaternion.LookRotation(lookRotation);
            transform.rotation = rotation;
        }


        public void PushEnemy(float pushForce, ForceMode forceMode)
        {
            if (canBePushed == false)
            {
                return;
            }

            var slamDirection = (transform.position - player.transform.position).normalized;
            slamDirection.y = 0;

            // Calculate the push force value, taking weight into account
            var pushForceValue = pushForce * (5f - (float)enemy.weight) * 3.5f;

            // Ensure the push force is not negative
            pushForceValue = Mathf.Max(pushForceValue, 1f);

            Vector3 targetPos = (pushForceValue) * slamDirection;

            MoveTowardsSmoothly(targetPos);
        }

        public void MoveTowardsSmoothly(Vector3 targetPos)
        {
            // Assign the target position to the force vector
            smoothDesiredDirection = targetPos;

            isMovingSmoothly = true;
        }

        public void BeginFall()
        {
            lastGroundedPositionY = transform.position.y;
        }
        public void StopFall()
        {
            var currentFallHeight = Mathf.Abs(lastGroundedPositionY - transform.position.y);

            // Takes fall damage?
            if (currentFallHeight > minimumFallHeightToTakeDamage && canFall
            // Just to have something that ensures this only triggers once
                && agent.enabled == false)
            {
                enemyHealthController.TakeEnvironmentalDamage(
                    Mathf.RoundToInt(currentFallHeight * damageMultiplierPerMeter));
            }
        }

        public void RepositionNavmeshAgent()
        {
            // Sample a valid position on the NavMesh near the animator's position
            Vector3 targetPosition = transform.position;
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, .5f, NavMesh.AllAreas))
            {
                targetPosition = hit.position;
            }

            // Disable the character controller temporarily to avoid conflicts
            characterController.enabled = false;

            // Teleport the agent to the target position smoothly
            agent.Warp(targetPosition);
            agent.updatePosition = true;

            // Re-enable the character controller
            characterController.enabled = true;
        }

        public void Revive()
        {
            if (enemyPushableOnDeath != null)
            {
                enemyPushableOnDeath.Deactivate();
            }



            animator.SetBool(hashDying, false);

            enemyHealthController.currentHealth = enemyHealthController.GetMaxHealth();
            enemyHealthController.EnableHealthHitboxes();
            enemyHealthController.InitializeEnemyHUD();
            
            if (customReviveTransformRef != null)
            {
                transform.position = customReviveTransformRef.transform.position;
                this.transform.rotation = customReviveTransformRef.transform.rotation;
            }
            else
            {
                transform.position = initialPosition;
                this.transform.rotation = initialRotation;
            }

            agent.enabled = true;


            if (!string.IsNullOrEmpty(customStartAnimation))
            {
                animator.Play(customStartAnimation);
            }
            else if (!string.IsNullOrEmpty(customReviveDefaultAnimation))
            {
                animator.Play(customReviveDefaultAnimation);
            }
            else
            {
                animator.Play(hashIdle);
            }

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

            if (enemyLoot != null)
            {
                enemyLoot.bonusGold = 0;
            }

            onRevive.Invoke();

            characterController.detectCollisions = true;
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

        public bool IsNavMeshAgentActive()
        {
            return agent.enabled && agent.isOnNavMesh;
        }


    }
}
