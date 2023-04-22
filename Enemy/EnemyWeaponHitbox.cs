using UnityEngine;

namespace AF
{

    public class EnemyWeaponHitbox : MonoBehaviour
    {
        public AudioClip weaponSwingSfx;
        public AudioClip weaponImpactSfx;

        public WeaponElementType weaponElementType = WeaponElementType.None;

        [Header("Bonus Stats")]
        public StatusEffect statusEffect;
        public int statusEffectAmountPerHit = 0;
        public int poiseDamage = 0;
        public int blockStaminaCost = 0;

        EnemyManager enemy;

        // References
        Collider boxCollider => GetComponent<Collider>();
        PlayerHealthbox playerHealthbox;
        DefenseStatManager defenseStatManager;
        HealthStatManager healthStatManager;
        PlayerParryManager playerParryManager;

        TrailRenderer trailRenderer;

        float maxTimerBeforeAllowingDamageAgain = .5f;
        float damageCooldownTimer = Mathf.Infinity;

        private void Awake()
        {
            enemy = GetComponentInParent<EnemyManager>();

            trailRenderer = GetComponent<TrailRenderer>();
            if (trailRenderer == null)
            {
                trailRenderer = GetComponentInChildren<TrailRenderer>();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            DisableHitbox();
        }

        private void OnEnable()
        {
            GetPlayerComponents();
        }

        private void Update()
        {

            if (damageCooldownTimer <= maxTimerBeforeAllowingDamageAgain)
            {
                damageCooldownTimer += Time.deltaTime;
            }
        }

        void GetPlayerComponents()
        {

            if (playerHealthbox == null)
            {
                playerHealthbox = FindObjectOfType<PlayerHealthbox>(true);
            }

            if (defenseStatManager == null)
            {
                defenseStatManager = FindObjectOfType<DefenseStatManager>(true);
            }

            if (healthStatManager == null)
            {
                healthStatManager = FindObjectOfType<HealthStatManager>(true);
            }

            if (playerParryManager == null)
            {
                playerParryManager = FindObjectOfType<PlayerParryManager>(true);
            }
        }

        public void EnableHitbox()
        {
            if (weaponSwingSfx != null)
            {
                BGMManager.instance.PlaySound(weaponSwingSfx, enemy.combatAudioSource);
            }


            boxCollider.enabled = true;

            if (trailRenderer != null)
            {
                trailRenderer.enabled = true;
                trailRenderer.emitting = true;
            }

        }

        public void DisableHitbox()
        {
            // Disable Tracking
            enemy.facePlayer = false;

            boxCollider.enabled = false;

            if (trailRenderer != null)
            {
                trailRenderer.emitting = false;
                //trailRenderer.enabled = false;
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("PlayerCompanionHealthHitbox"))
            {
                other.GetComponentInParent<CompanionManager>().TakeDamage(enemy.enemyCombatController.GetCurrentAttack(), enemy);

                return;
            }

            if (!other.gameObject.CompareTag("Player"))
            {
                return;
            }

            GetPlayerComponents();

            if (damageCooldownTimer <= maxTimerBeforeAllowingDamageAgain)
            {
                return;
            }

            if (playerParryManager.IsParrying() && enemy.enemyPostureController != null && enemy.enemyPostureController.isParriable)
            {
                playerParryManager.InstantiateParryFx();
                enemy.enemyPostureController.TakePostureDamage();
                return;
            }

            float damageToReceive = Mathf.Clamp(
                enemy.enemyCombatController.GetCurrentAttack() - defenseStatManager.GetDefenseAbsorption(),
                Random.Range(1, 10),
                healthStatManager.GetMaxHealth()
            );

            if (weaponElementType == WeaponElementType.Magic)
            {
                damageToReceive -= (int)defenseStatManager.GetMagicDefense();
            }

            playerHealthbox.TakeDamage(damageToReceive, enemy.transform, weaponImpactSfx, enemy.enemyCombatController.currentAttackPoiseDamage);

            DisableHitbox();

            damageCooldownTimer = 0f;
        }
    }
}
