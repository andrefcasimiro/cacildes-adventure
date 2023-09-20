using UnityEngine;

namespace AF
{

    public class EnemyWeaponHitbox : MonoBehaviour
    {
        public AudioClip weaponSwingSfx;
        public AudioClip weaponImpactSfx;

        public int weaponElementalDamage = 0;
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

        [Header("Hitbox Special FX")]
        public DestroyableParticle hitboxSpecialFx;
        public Transform hitboxTransformRef;

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

        public void ActivateHitboxSpecial()
        {
            if (hitboxSpecialFx == null)
            {
                return;
            }

            Vector3 pos = transform.position;
            if (hitboxTransformRef != null)
            {
                pos = hitboxTransformRef.transform.position;
            }

            Instantiate(hitboxSpecialFx, pos, Quaternion.identity);
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


            if (enemy.enemyCombatController.isDamagingHimself)
            {
                enemy.enemyHealthController.TakeEnvironmentalDamage(50f);
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

        public bool IsActive()
        {
            return boxCollider.enabled;
        }

        private void OnTriggerStay(Collider other)
        {

            if (other.CompareTag("Explodable"))
            {
                other.TryGetComponent(out ExplodingBarrel explodingBarrel);

                if (explodingBarrel != null)
                {
                    explodingBarrel.Explode();
                }
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

            if (playerParryManager.IsParrying() && enemy.enemyPostureController != null)
            {
                playerParryManager.InstantiateParryFx();

                int parryDamage = 0;
                int playerParryBonus = (int)playerParryManager.GetComponent<EquipmentGraphicsHandler>().parryPostureDamageBonus;
                if (playerParryBonus > 0)
                {
                    parryDamage = enemy.enemy.postureDamagePerParry + playerParryBonus;
                }

                enemy.enemyPostureController.TakePostureDamage(parryDamage);
                return;
            }

            float damageToReceive = CalculateDamageToReceive(enemy.enemyCombatController.GetCurrentAttack(),
                defenseStatManager.GetDefenseAbsorption(), healthStatManager.GetMaxHealth());

            float elementalDamage = Player.instance.CalculateIncomingElementalAttack((int)weaponElementalDamage, weaponElementType, defenseStatManager);

            if (!string.IsNullOrEmpty(enemy.enemyCombatController.playerExecutedClip) && !string.IsNullOrEmpty(enemy.enemyCombatController.transitionToExecution))
            {
                enemy.animator.Play(enemy.enemyCombatController.transitionToExecution);

                // Calculate the desired position in front of the enemy
                Vector3 desiredPlayerPosition = enemy.transform.position + enemy.transform.forward * 1f;

                // Ensure the player stays at the same height as the enemy (if required)
                desiredPlayerPosition.y = enemy.transform.position.y;

                // Move the player to the desired position and rotation
                enemy.player.GetComponent<PlayerComponentManager>().UpdatePosition(desiredPlayerPosition, enemy.transform.rotation);
                enemy.player.GetComponent<PlayerComponentManager>().DisableComponents();
                enemy.player.GetComponent<Animator>().Play(enemy.enemyCombatController.playerExecutedClip);
                enemy.player.GetComponent<PlayerPoiseController>().MarkAsStunned();
            }


            playerHealthbox.TakeDamage(damageToReceive, enemy.transform, weaponImpactSfx, enemy.enemyCombatController.currentAttackPoiseDamage, (int)elementalDamage, weaponElementType);

            DisableHitbox();

            damageCooldownTimer = 0f;
        }

        float CalculateDamageToReceive(float currentAttack, float defenseAbsorption, float maxHealth)
        {
            // Calculate the raw damage before absorption
            float rawDamage = Mathf.Max(1f, currentAttack - defenseAbsorption);

            // Calculate the logarithmic scaling factor based on defense absorption
            float scalingFactor = 1f / (1f + Mathf.Log(defenseAbsorption + 1f, 20f));

            // Calculate the final damage to receive
            float damageToReceive = rawDamage * scalingFactor;

            // Make sure the damage is at least 1 to avoid zero or negative values
            damageToReceive = Mathf.Max(damageToReceive, 1f);

            // Clamp the damage to not exceed the player's maximum health
            damageToReceive = Mathf.Clamp(damageToReceive, 1f, maxHealth);

            return (int)damageToReceive;
        }
    }
}
