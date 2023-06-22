using UnityEngine;

namespace AF
{
    public class CompanionWeaponHitbox : MonoBehaviour
    {
        public AudioClip weaponSwingSfx;
        public AudioClip weaponImpactSfx;
        public int poiseDamage = 1;

        CompanionManager companionManager;

        // References
        Collider boxCollider => GetComponent<Collider>();
        public TrailRenderer trailRenderer;

        float maxTimerBeforeAllowingDamageAgain = .5f;
        float damageCooldownTimer = Mathf.Infinity;

        public StatusEffect statusEffectToInflict;
        public int amountOfStatusEffectPerHit;

        public int pushForceAmount = -1;

        private void Awake()
        {
            companionManager = GetComponentInParent<CompanionManager>();
        }

        void Start()
        {
            DisableHitbox();
        }

        private void Update()
        {
            if (damageCooldownTimer <= maxTimerBeforeAllowingDamageAgain)
            {
                damageCooldownTimer += Time.deltaTime;
            }
        }

        public void EnableHitbox()
        {
            if (weaponSwingSfx != null)
            {
                BGMManager.instance.PlaySound(weaponSwingSfx, companionManager.combatAudioSource);
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
            if (!other.TryGetComponent<EnemyHealthHitbox>(out var enemy))
            {
                return;
            }

            if (damageCooldownTimer <= maxTimerBeforeAllowingDamageAgain)
            {
                return;
            }

            if (enemy.enemyManager.enemyHealthController.currentHealth <= 0)
            {
                return;
            }

            enemy.enemyManager.enemyHealthController.TakeEnvironmentalDamage(companionManager.GetCompanionAttack());
            enemy.enemyManager.enemyPoiseController.IncreasePoiseDamage(poiseDamage);

            if (statusEffectToInflict != null)
            {

                var enemyStatusController = enemy.enemyManager.enemyNegativeStatusController;

                if (enemyStatusController != null)
                {
                    enemyStatusController.InflictStatusEffect(statusEffectToInflict, amountOfStatusEffectPerHit);
                }
            }

            if (enemy.enemyManager.enemyTargetController != null)
            {
                enemy.enemyManager.enemyTargetController.FocusOnCompanion(this.companionManager);
            }

            if (pushForceAmount != -1)
            {
                enemy.enemyManager.PushEnemy(pushForceAmount, ForceMode.Acceleration);
            }

            BGMManager.instance.PlaySound(weaponImpactSfx, companionManager.combatAudioSource);

            DisableHitbox();

            damageCooldownTimer = 0f;
        }
    }
}
