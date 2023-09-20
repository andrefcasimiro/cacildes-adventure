using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace AF
{
    public class CompanionManager : MonoBehaviour
    {
        public Companion companion;

        [TextArea]
        public string comment = "Companions must be isKinematic for EventNavigatorCapturable to work smoothly";
        
        #region Animation Hashes
        public readonly int hashIdle = Animator.StringToHash("Idle");

        public readonly int hashInCombat = Animator.StringToHash("InCombat");

        public readonly int hashChasing = Animator.StringToHash("Chasing");
        public readonly int hashIsChasingEnemy = Animator.StringToHash("IsChasingEnemy");

        public readonly int hashIsCombatting = Animator.StringToHash("IsCombatting");

        public readonly int hashWalkToPlayer = Animator.StringToHash("WalkToPlayer");
        public readonly int hashRunToPlayer = Animator.StringToHash("RunToPlayer");

        public readonly int hashIsAttacking = Animator.StringToHash("IsAttacking");

        public readonly int hashIsInjured = Animator.StringToHash("IsInjured");
        public readonly int hashInjured = Animator.StringToHash("Injured");

        public readonly int hashTakeDamage = Animator.StringToHash("TakeDamage");

        #endregion

        [Header("Walk Speed")]
        public float walkSpeed = 2f;

        [Header("Run To Player")]
        public float runSpeed = 3.5f;
        public float minimumDistanceToRunToPlayer = 5;

        [Header("Cooldown Between Attacks")]
        public float maxCooldownBetweenAttacks = 7.5f;
        float cooldownBetweenAttacks = Mathf.Infinity;

        [Header("Behaviour")]
        public bool inParty = false;
        public bool waitingForPlayer = false;

        [Header("Combat")]
        public int baseAttackPower = 50;
        public AudioSource combatAudioSource;
        [Range(0, 100)]
        public float attackWeight = 75;
        public CompanionWeaponHitbox leftWeapon;
        public CompanionWeaponHitbox rightWeapon;

        [Header("Health")]
        public float baseHealth = 100;
        public float currentHealth = 0;
        public float maxInjuredCooldown = 15f;
        float injuredCooldown = Mathf.Infinity;
        public DestroyableParticle hurtFx;

        // Components
        Animator animator => GetComponent<Animator>();
        [HideInInspector] public NavMeshAgent agent => GetComponent<NavMeshAgent>();
         public GameObject player;
        PlayerLevelManager playerLevelManager;

        // Internal references
        public EnemyManager currentEnemy;

        [HideInInspector] public float defaultStoppingDistance;

        [Header("Scene Settings")]
        [Tooltip("If true, this companion will spawn in this scene even if not in party as this scene will actual as his/her home")]
        public bool sceneIsHome = false;

        [Header("Far Away Respawn")]
        public DestroyableParticle spawnCompanionFx;
        public Vector3 previousPosition;
        public float maxTimeStuck = 10f;
        float timeInSamePosition = 0f;
        public int maxDistanceToPlayerBeforeRespawning = 15;


        private void Awake()
        {
            player = FindAnyObjectByType<PlayerCombatController>(FindObjectsInactive.Include).gameObject;

            transform.position = player.transform.position;


            defaultStoppingDistance = agent.stoppingDistance;
        }

        void Start()
        {

            playerLevelManager = player.GetComponent<PlayerLevelManager>();

            InitializeHealth();

        }

        private void Update()
        {
            UpdateAttackCooldown();

            UpdateInjuredCooldown();

            CheckIfPlayerIsUnreachable();
        }

        #region Combat
        void UpdateAttackCooldown()
        {
            if (cooldownBetweenAttacks < maxCooldownBetweenAttacks)
            {
                cooldownBetweenAttacks += Time.deltaTime;
            }
        }

        public void FaceEnemy()
        {
            if (waitingForPlayer || inParty == false || currentEnemy == null)
            {
                return;
            }

            var lookRotation = currentEnemy.transform.position - this.transform.position;
            var rotation = Quaternion.LookRotation(lookRotation);

            transform.rotation = rotation;
        }

        public int GetCompanionAttack()
        {
            return Player.instance.CalculateAIGenericValue(baseAttackPower, playerLevelManager.GetCurrentLevel());
        }

        public void ResetAttackCooldown()
        {
            // add some unpredictability
            var startPoint = Random.Range(0, 1);

            cooldownBetweenAttacks = startPoint;
        }

        public bool CanAttack()
        {
            if (cooldownBetweenAttacks < maxCooldownBetweenAttacks)
            {
                return false;
            }

            var dice = Random.Range(0, 100);

            if (dice > attackWeight)
            {
                return false;
            }

            return true;
        }

        public void StopCombat()
        {
            if (waitingForPlayer || inParty == false)
            {
                return;
            }

            this.currentEnemy = null;
            animator.Play(hashIdle);
        }

        public void ForceIntoCombat(EnemyManager currentEnemy)
        {
            if (waitingForPlayer || inParty == false)
            {
                return;
            }

            // Check if companion is near first


            this.currentEnemy = currentEnemy;

            animator.Play(hashChasing);
        }

        public bool IsInCombat()
        {
            return animator.GetBool(hashInCombat);
        }

        #endregion

        #region Weapon Hitboxes

        public void DisableAllWeaponHitboxes()
        {
            DeactivateLeftHandHitbox();
            DeactivateRightHandHitbox();
        }


        public void ActivateLeftHandHitbox()
        {
            if (leftWeapon == null)
            {
                return;
            }

            leftWeapon.EnableHitbox();
        }
        public void DeactivateLeftHandHitbox()
        {
            if (leftWeapon == null)
            {
                return;
            }

            leftWeapon.DisableHitbox();
        }

        public void ActivateRightHandHitbox()
        {
            if (rightWeapon == null)
            {
                return;
            }

            rightWeapon.EnableHitbox();
        }

        public void DeactivateRightHandHitbox()
        {
            if (rightWeapon == null)
            {
                return;
            }

            rightWeapon.DisableHitbox();
        }

        public void ActivateLeftWeaponHitbox() { ActivateLeftHandHitbox(); }
        public void DeactivateLeftWeaponHitbox() { DeactivateLeftHandHitbox(); }

        public void ActivateRightWeaponHitbox() { ActivateRightHandHitbox(); }
        public void DeactivateRightWeaponHitbox() { DeactivateRightHandHitbox(); }

        #endregion

        #region Chase Enemy

        public bool ShouldChaseEnemy()
        {
            if (waitingForPlayer || inParty == false)
            {
                return false;
            }

            if (currentEnemy == null)
            {
                return false;
            }

            // If already chasing
            if (animator.GetBool(hashIsChasingEnemy))
            {
                return false;
            }

            return Vector3.Distance(transform.position, currentEnemy.transform.position) > agent.stoppingDistance;
        }

        public bool IsEnemyFarAway()
        {
            if (currentEnemy == null)
            {
                return false;
            }

            return Vector3.Distance(transform.position, currentEnemy.transform.position) > agent.stoppingDistance;
        }

        #endregion

        #region Player Follow Logic
        public void CheckIfPlayerIsUnreachable()
        {
            if (!ShouldRunToPlayer())
            {
                return;
            }

            if (Vector3.Distance(player.transform.position, transform.position) >= maxDistanceToPlayerBeforeRespawning)
            {
                RespawnNearPlayer();
                timeInSamePosition = 0f;
            }
            else if (Mathf.Round(previousPosition.x) == Mathf.Round(transform.position.x) && Mathf.Round(previousPosition.y) == Mathf.Round(transform.position.y) &&
                    Mathf.Round(previousPosition.z) == Mathf.Round(transform.position.z))
            {
                timeInSamePosition += Time.deltaTime;

                if (timeInSamePosition > maxTimeStuck)
                {
                    RespawnNearPlayer();
                    timeInSamePosition = 0f;
                }
            }
            else
            {
                timeInSamePosition = 0f;
            }

            // Should run to player, but is stuck
            previousPosition = transform.position;
        }

        public void RespawnNearPlayer()
        {
            currentEnemy = null;

            Transform targetTransform = player.transform;

            // Teleport near player
            NavMesh.SamplePosition(targetTransform.position + targetTransform.forward * -2f, out NavMeshHit rightHit, 10f, NavMesh.AllAreas);

            if (rightHit.hit)
            {
                agent.enabled = false;
                agent.nextPosition = rightHit.position;
                transform.position = rightHit.position;
                agent.enabled = true;

                Instantiate(spawnCompanionFx, transform.position, Quaternion.identity);
            }

        }

        public bool ShouldRunToPlayer()
        {
            if (waitingForPlayer || inParty == false)
            {
                return false;
            }

            return Vector3.Distance(transform.position, player.transform.position) > agent.stoppingDistance + minimumDistanceToRunToPlayer;
        }

        public bool ShouldWalkToPlayer()
        {
            if (waitingForPlayer || inParty == false)
            {
                return false;
            }

            return ShouldRunToPlayer() == false && Vector3.Distance(transform.position, player.transform.position) > agent.stoppingDistance;
        }
        #endregion

        #region Health

        void InitializeHealth()
        {
            this.currentHealth = GetMaxHealth();
        }

        public void TakeDamage(float damage, EnemyManager enemyManager)
        {
            // If enemy and companion not facing each other, dont take damage to avoid receiving hitbox damages that are not correct
            if (Vector3.Angle(transform.forward * -1, enemyManager.transform.forward) > 90f)
            {
                return;
            }

            var damageToReceive = Mathf.Clamp(damage, 0, GetMaxHealth());

            this.currentHealth -= (damageToReceive / 2);

            Instantiate(hurtFx, transform.position, Quaternion.identity);

            if (this.currentHealth <= 0)
            {
                animator.Play(hashInjured);
                injuredCooldown = 0;

                if (enemyManager.enemyTargetController != null)
                {
                    enemyManager.enemyTargetController.BreakCompanionFocus();
                }
            }
            else
            {
                animator.Play(hashTakeDamage);
            }
        }

        void UpdateInjuredCooldown()
        {
            if (injuredCooldown < maxInjuredCooldown)
            {
                injuredCooldown += Time.deltaTime;
            }
            else if (injuredCooldown >= maxInjuredCooldown && animator.GetBool(hashIsInjured))
            {
                this.currentHealth = GetMaxHealth();
                animator.SetBool(hashIsInjured, false);
            }
        }

        public bool IsInjured()
        {
            return animator.GetBool(hashIsInjured);
        }

        public int GetMaxHealth()
        {
            return Player.instance.CalculateAIGenericValue((int)baseHealth, playerLevelManager.GetCurrentLevel());
        }

        #endregion

    }

}
