using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

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

        [Header("On Killing Events")]
        public float experienceGained = 0f;

        [Header("Projectile Options")]
        public GameObject projectilePrefab;
        public Transform projectileSpawnPoint;
        public float projectileCooldown = 0f;
        public float maxProjectileCooldown = 10f;

        [Header("Weapon Settings")]
        public AudioSource combatAudioSource;
        public EnemyWeaponInstance weapon;
        #region Weapon Attack Stats (Set or influenced by every attack animation clip);
        public float weaponDamage = 100f;
        public StatusEffect weaponStatusEffect = null;
        public float statusEffectAmount = 0f;
        #endregion
        [Range(0, 1)] public float heavyAttackFrequency = 0.85f;

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
            foreach (Transform waypointChild in waypointsParent.transform)
            {
                this.waypoints.Add(waypointChild);
            }

            maxHealth = this.health;
        }

        private void Start()
        {
            player = GameObject.FindWithTag("Player").GetComponent<Player>();
        }

        public void GotoNextPoint()
        {
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

        #region Animation Events
        public void FireProjectile()
        {
            Utils.FaceTarget(this.transform, player.transform);

            GameObject projectileInstance = Instantiate(projectilePrefab, this.projectileSpawnPoint.transform.position, Quaternion.identity);

            Projectile projectile = projectileInstance.GetComponent<Projectile>();

            projectile.Shoot(player.headTransform);
        }

        public void ActivateHitbox()
        {
            if (weapon == null)
            {
                return;
            }

            weapon.EnableHitbox();
        }

        public void DeactivateHitbox()
        {
            if (weapon == null)
            {
                return;
            }

            weapon.DisableHitbox();
        }

        public void PlayGroundImpact()
        {
            if (groundImpactSfx == null)
            {
                return;
            }

            combatAudioSource.PlayOneShot(groundImpactSfx);
        }

        #endregion

        public IWeaponInstance GetWeaponInstance()
        {
            return this.weapon;
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
    }
}
