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
            if (other.CompareTag("Explodable"))
            {
                other.TryGetComponent(out ExplodingBarrel explodingBarrel);

                if (explodingBarrel != null)
                {
                    explodingBarrel.Explode();
                    return;
                }
            }

            CharacterHealthHitbox enemyHealthHitbox = other.GetComponent<CharacterHealthHitbox>();

            if (enemyHealthHitbox == null)
            {
                enemyHealthHitbox = other.GetComponentInChildren<CharacterHealthHitbox>();

            }

            if (enemyHealthHitbox == null)
            {
                return;
            }

            if (damageCooldownTimer <= maxTimerBeforeAllowingDamageAgain)
            {
                return;
            }

            /*if (enemyHealthHitbox.characterManager.enemyHealthController.currentHealth <= 0)
            {
                return;
            }

            enemyHealthHitbox.characterManager.enemyHealthController.TakeEnvironmentalDamage(companionManager.GetCompanionAttack());
            enemyHealthHitbox.characterManager.enemyPoiseController.IncreasePoiseDamage(poiseDamage);
*/
            if (statusEffectToInflict != null)
            {

                /*var enemyStatusController = enemyHealthHitbox.characterManager.enemyNegativeStatusController;

                if (enemyStatusController != null)
                {
                    enemyStatusController.InflictStatusEffect(statusEffectToInflict, amountOfStatusEffectPerHit);
                }*/
            }


            /*            if (enemyHealthHitbox.characterManager.enemyTargetController != null)
                        {
                            enemyHealthHitbox.characterManager.enemyTargetController.FocusOnCompanion(this.companionManager);
                        }

                        if (pushForceAmount != -1)
                        {
                            enemyHealthHitbox.characterManager.PushEnemy(pushForceAmount, ForceMode.Acceleration);
                        }*/

            BGMManager.instance.PlaySound(weaponImpactSfx, companionManager.combatAudioSource);

            DisableHitbox();

            damageCooldownTimer = 0f;
        }
    }
}
