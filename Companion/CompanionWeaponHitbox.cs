using UnityEngine;

namespace AF
{
    public class CompanionWeaponHitbox : MonoBehaviour
    {
        public AudioClip weaponSwingSfx;
        public AudioClip weaponImpactSfx;

        CompanionManager companionManager;

        // References
        Collider boxCollider => GetComponent<Collider>();
        TrailRenderer trailRenderer => GetComponent<TrailRenderer>();

        float maxTimerBeforeAllowingDamageAgain = .5f;
        float damageCooldownTimer = Mathf.Infinity;

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
            if (other.gameObject.tag != "EnemyHealthHitbox")
            {
                return;
            }

            var enemy = other.GetComponent<EnemyHealthHitbox>();
            if (enemy == null)
            {
                return;
            }

            if (damageCooldownTimer <= maxTimerBeforeAllowingDamageAgain)
            {
                return;
            }

            enemy.enemyManager.enemyHealthController.TakeEnvironmentalDamage(companionManager.GetCompanionAttack());
            
            if (enemy.enemyManager.enemyTargetController != null)
            {
                enemy.enemyManager.enemyTargetController.FocusOnCompanion(this.companionManager);
            }

            BGMManager.instance.PlaySound(weaponImpactSfx, companionManager.combatAudioSource);

            DisableHitbox();

            damageCooldownTimer = 0f;
        }
    }
}
