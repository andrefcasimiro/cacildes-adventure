using UnityEngine;

namespace AF
{

    [RequireComponent(typeof(Animator))]
    public class EnemyCombatManager : MonoBehaviour, ICombatable
    {
        public float health = 100f;
        float maxHealth;

        public AudioSource combatAudioSource;

        [Header("Combat Sounds")]
        public AudioClip weaponSwingSfx;
        public AudioClip dodgeSfx;
        public AudioClip groundImpactSfx;
        public AudioClip clothSfx;

        [Header("Projectile Options")]
        public GameObject projectilePrefab;
        public Transform projectileSpawnPoint;

        [Header("Weapon Options")]
        public EnemyWeaponInstance weapon;
        public ShieldInstance shield;

        Player player;

        private void Awake()
        {
            maxHealth = this.health;
        }

        private void Start()
        {
            this.player = GameObject.FindWithTag("Player").GetComponent<Player>();
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

        public void FireProjectile()
        {
            Utils.FaceTarget(this.transform, player.transform);

            GameObject projectileInstance = Instantiate(projectilePrefab, this.projectilePrefab.transform.position, Quaternion.identity);

            Projectile projectile = projectileInstance.GetComponent<Projectile>();

            Utils.PlaySfx(combatAudioSource, projectile.projectileSfx);
            projectile.Shoot(player.headTransform);
        }

        public IWeaponInstance GetWeaponInstance()
        {
            return this.weapon;
        }

        public ShieldInstance GetShieldInstance()
        {
            return this.shield;
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
