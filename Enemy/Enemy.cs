using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.Events;

namespace AF
{

    [RequireComponent(typeof(NavMeshAgent))]
    public class Enemy : Character, ICombatable
    {

        // State Machine Constants
        public readonly int hashIdle = Animator.StringToHash("Idle");
        public readonly int hashPatrol = Animator.StringToHash("Patrolling");
        public readonly int hashChasing = Animator.StringToHash("Chasing");
        public readonly int hashCombatting = Animator.StringToHash("Combatting");
        public readonly int hashShooting = Animator.StringToHash("Shooting");
        public readonly int hashBuff = Animator.StringToHash("Buffing");

        [Header("General Settings")]
        public float rotationSpeed = 5f;

        [Header("Health Settings")]
        public float health = 100f;
        float maxHealth;

        [Header("Patrolling")]
        public Transform waypointsParent;
        public List<Transform> waypoints = new List<Transform>();
        public float restingTimeOnWaypoint = 2f;
        int destinationPoint = 0;

        [Header("Chasing")]
        public float maximumChaseDistance = 10f;

        [Header("Sight")]
        public float fovAngle = 110f;
        public float sightDistance = 20f;
        public LayerMask obstructionMask;

        [Header("On Buff Events")]
        public UnityEvent onBuffEvent;
        [Range(0, 100)]
        public float maxHealthPercentageBeforeBuff;
        [Range(0, 100)]
        public float minimumChanceToUseBuff;
        public float maxBuffCooldown = 15f;
        float buffCooldown = 0f;

        [Header("On Killing Events")]
        public UnityEvent onEnemyDeath;
        public float experienceGained = 0f;

        [Header("Projectile Options")]
        public GameObject projectilePrefab;
        public Transform projectileSpawnPoint;
        public float projectileCooldown = 0f;
        public float maxProjectileCooldown = 10f;

        [Header("Weapon Settings")]
        public AudioSource combatAudioSource;
        
        // All Possible Weapon Hitboxes Instances
        public EnemyWeaponInstance leftHandWeapon;
        public EnemyWeaponInstance rightHandWeapon;
        public EnemyWeaponInstance leftLegWeapon;
        public EnemyWeaponInstance rightLegWeapon;
        public EnemyWeaponInstance headWeapon;
        public EnemyWeaponInstance areaOfImpactWeapon;
        [Header("Area Of Impact FX")]
        public GameObject areaOfImpactFX;
        public Transform areaOfImpactTransform;

        #region Weapon Attack Stats (Set or influenced by every attack animation clip);
        public float weaponDamage = 100f;
        public StatusEffect weaponStatusEffect = null;
        public float statusEffectAmount = 0f;
        public float bonusBlockStaminaCost = 0f;
        #endregion
        [Range(0, 1)] public float heavyAttackFrequency = 0.85f;
        public bool isWaiting = false;
        public float turnWaitingTime = 0f;
        public float maxWaitingTimeBeforeResumingCombat = 2f;
        protected float waitingCounter = 0f;


        [Header("Block Settings")]
        public GameObject shield;
        public ShieldInstance shieldInstance;
        public DestroyableParticle blockParticleEffect;
        [Range(0, 1)] public float blockFrequency = 0.85f;

        [Header("Dodge Settings")]
        public AudioClip dodgeSfx;
        [Range(0, 1)] public float dodgeFrequency = 0.85f;

        [Header("Ground Settings")]
        public AudioClip groundImpactSfx;

        // Components
        [HideInInspector] public NavMeshAgent agent => GetComponent<NavMeshAgent>();

        // References
        [HideInInspector] public Player player;

        // Flags
        [HideInInspector] public bool facePlayer = false;

        private void Awake()
        {
            if (waypointsParent != null)
            {
                foreach (Transform waypointChild in waypointsParent.transform)
                {
                    this.waypoints.Add(waypointChild);
                }
            }

            maxHealth = this.health;
        }

        private void Start()
        {
            player = GameObject.FindWithTag("Player").GetComponent<Player>();
        }

        public void GotoNextPoint()
        {
            if (waypoints.Count <= 0)
            {
                return;
            }

            agent.destination = waypoints[destinationPoint].position;
            destinationPoint = (destinationPoint + 1) % waypoints.Count;
        }

        private void Update()
        {
            if (facePlayer)
            {
                var lookRotation = player.transform.position - this.transform.position;
                var rotation = Quaternion.LookRotation(lookRotation);
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rotation, Time.deltaTime * rotationSpeed);
            }

            if (animator.GetBool(hashChasing) && projectilePrefab != null)
            {
                projectileCooldown += Time.deltaTime;
            }

            if (isWaiting)
            {
                waitingCounter += Time.deltaTime;

                if (waitingCounter >= turnWaitingTime && animator.GetBool(hashCombatting) == false)
                {
                    waitingCounter = 0f;
                    turnWaitingTime = 0f;
                    isWaiting = false;
                    animator.SetBool(hashCombatting, true);
                }
            }

            HandleBuff();
        }

        void HandleBuff()
        {

            float currentHealthPercentage = GetCurrentHealth() * 100 / GetMaxHealth();

            if (currentHealthPercentage > maxHealthPercentageBeforeBuff)
            {
                return;
            }

            buffCooldown += Time.deltaTime;
            if (buffCooldown < maxBuffCooldown)
            {
                return;
            }


            buffCooldown = 0f;

            float chanceToBuff = Random.RandomRange(0, 100f);
            if (chanceToBuff < minimumChanceToUseBuff)
            {
                return;
            }

            animator.Play(hashBuff);
        }

        public bool CanShoot()
        {
            return projectilePrefab != null && projectileCooldown >= maxProjectileCooldown;
        }

        public void PrepareProjectile()
        {
            animator.SetTrigger(hashShooting);
            projectileCooldown = 0f;
        }


        public IWeaponInstance GetWeaponInstance()
        {
            return null;
        }

        public ShieldInstance GetShieldInstance()
        {
            return this.shieldInstance;
        }

        public AudioSource GetCombatantAudioSource()
        {
            return this.combatAudioSource;
        }

        public float GetCurrentHealth()
        {
            return this.health;
        }

        public float GetMaxHealth()
        {
            return this.maxHealth;
        }

        public void SetCurrentHealth(float health)
        {
            this.health = Mathf.Clamp(health, 0f, GetMaxHealth());
        }

        #region Animation Events
        public void CastBuff()
        {
            this.onBuffEvent.Invoke();
        }

        public void FireProjectile()
        {
            Utils.FaceTarget(this.transform, player.transform);

            GameObject projectileInstance = Instantiate(projectilePrefab, this.projectileSpawnPoint.transform.position, Quaternion.identity);

            Projectile projectile = projectileInstance.GetComponent<Projectile>();

            projectile.Shoot(player.headTransform);
        }

        public void PlayGroundImpact()
        {
            if (groundImpactSfx == null)
            {
                return;
            }

            combatAudioSource.PlayOneShot(groundImpactSfx);
        }

        public void ActivateLeftHandHitbox()
        {
            if (leftHandWeapon == null)
            {
                return;
            }

            leftHandWeapon.EnableHitbox();
        }
        public void DeactivateLeftHandHitbox()
        {
            if (leftHandWeapon == null)
            {
                return;
            }

            leftHandWeapon.DisableHitbox();
        }

        public void ActivateRightHandHitbox()
        {
            if (rightHandWeapon == null)
            {
                return;
            }

            rightHandWeapon.EnableHitbox();
        }

        public void DeactivateRightHandHitbox()
        {
            if (rightHandWeapon == null)
            {
                return;
            }

            rightHandWeapon.DisableHitbox();
        }

        public void ActivateRightLegHitbox()
        {
            if (rightLegWeapon == null)
            {
                return;
            }

            rightLegWeapon.EnableHitbox();
        }

        public void DeactivateRightLegHitbox()
        {
            if (rightLegWeapon == null)
            {
                return;
            }

            rightLegWeapon.DisableHitbox();
        }

        public void ActivateLeftLegHitbox()
        {
            if (leftLegWeapon == null)
            {
                return;
            }

            leftLegWeapon.EnableHitbox();
        }

        public void DeactivateLeftLegHitbox()
        {
            if (leftLegWeapon == null)
            {
                return;
            }

            leftLegWeapon.DisableHitbox();
        }

        public void ActivateHeadHitbox()
        {
            if (headWeapon == null)
            {
                return;
            }

            headWeapon.EnableHitbox();
        }

        public void DeactivateHeadHitbox()
        {
            if (headWeapon == null)
            {
                return;
            }

            headWeapon.DisableHitbox();
        }

        public void ActivateAreaOfImpactHitbox()
        {
            if (areaOfImpactWeapon == null)
            {
                return;
            }

            Instantiate(areaOfImpactFX, areaOfImpactTransform);

            areaOfImpactWeapon.EnableHitbox();
        }

        public void DeactivateAreaOfImpactHitbox()
        {
            if (areaOfImpactWeapon == null)
            {
                return;
            }

            areaOfImpactWeapon.DisableHitbox();
        }

        #endregion
    }
}
