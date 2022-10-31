using UnityEngine;

namespace AF
{

    public class EnemyWeaponHitbox : MonoBehaviour
    {
        public AudioClip weaponSwingSfx;
        public AudioClip weaponImpactSfx;

        Enemy enemy;
        EnemyCombatController enemyCombatController;
        EnemyPostureController enemyPostureController;

        AudioSource combatantAudioSource;

        // References
        Collider boxCollider => GetComponent<Collider>();
        PlayerHealthbox playerHealthbox;
        DefenseStatManager defenseStatManager;
        HealthStatManager healthStatManager;
        PlayerParryManager playerParryManager;

        TrailRenderer trailRenderer => GetComponent<TrailRenderer>();

        float maxTimerBeforeAllowingDamageAgain = .5f;
        float damageCooldownTimer = Mathf.Infinity;

        private void Awake()
        {
            enemy = GetComponentInParent<Enemy>();
            enemyCombatController = enemy.GetComponent<EnemyCombatController>();
            enemyPostureController = enemy.GetComponent<EnemyPostureController>();

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
            if (weaponSwingSfx != null && combatantAudioSource != null)
            {
                Utils.PlaySfx(combatantAudioSource, weaponSwingSfx);
            }

            boxCollider.enabled = true;

            if (trailRenderer != null)
            {
                trailRenderer.enabled = true;
            }

        }

        public void DisableHitbox()
        {
            boxCollider.enabled = false;

            if (trailRenderer != null)
            {
                trailRenderer.enabled = false;
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != "Player")
            {
                return;
            }
            
            GetPlayerComponents();

            float damageToReceive = Mathf.Clamp(
                enemyCombatController.weaponDamage - defenseStatManager.GetDefenseAbsorption(),
                1f,
                healthStatManager.GetMaxHealth()
            );

            if (damageCooldownTimer <= maxTimerBeforeAllowingDamageAgain)
            {
                return;
            }

            if (playerParryManager.IsParrying())
            {
                playerParryManager.InstantiateParryFx();
                enemyPostureController.TakePostureDamage();
                return;
            }

            playerHealthbox.TakeDamage(damageToReceive, enemy.transform, weaponSwingSfx);

            DisableHitbox();

            damageCooldownTimer = 0f;
        }

    }

}
