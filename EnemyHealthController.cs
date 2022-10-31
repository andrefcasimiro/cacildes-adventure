using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace AF
{
    public class EnemyHealthController : MonoBehaviour
    {
        public readonly int hashDying = Animator.StringToHash("Dying");
        public readonly int hashIsDead = Animator.StringToHash("Dead");

        [Header("Stats")]
        [Tooltip("Don't change this value. It will start with the maxHealth value.")]
        public float currentHealth;
        public float maxHealth = 100f;

        [Header("UI")]
        public Slider healthBarSlider;
        public FloatingText damageFloatingText;

        [Header("Visuals")]
        public DestroyableParticle damageParticlePrefab;

        [Header("Sounds")]
        public AudioClip deathGruntSfx;

        [Header("On Killing Events")]
        public UnityEvent onEnemyDeath;

        // Components
        Enemy enemy => GetComponent<Enemy>();
        EnemyCombatController enemyCombatController => GetComponent<EnemyCombatController>();
        EnemyPoiseController enemyPoiseController => GetComponent<EnemyPoiseController>();
        EnemyBlockController enemyBlockController => GetComponent<EnemyBlockController>();
        EnemyElementalDamageController enemyElementalDamageController => GetComponent<EnemyElementalDamageController>();
        EnemyStatusManager enemyStatusManager => GetComponent<EnemyStatusManager>();
        EnemyPostureController enemyPostureController => GetComponent<EnemyPostureController>();

        private EnemyHealthHitbox[] enemyHealthHitboxes;

        LockOnManager lockOnManager;
        LockOnRef lockOnRef;

        private void Start()
        {
            lockOnManager = FindObjectOfType<LockOnManager>(true);
            lockOnRef = GetComponentInChildren<LockOnRef>(true);

            currentHealth = maxHealth;

            this.enemyHealthHitboxes = GetComponentsInChildren<EnemyHealthHitbox>(true);

            if (healthBarSlider != null)
            {
                healthBarSlider.maxValue = maxHealth * 0.01f;
                healthBarSlider.value = currentHealth * 0.01f;
            }
        }

        private void Update()
        {
            if (healthBarSlider != null)
            {
                healthBarSlider.value = currentHealth * 0.01f;
            }
        }

        public void EnableHealthHitboxes()
        {
            foreach (var enemyHealthHitbox in enemyHealthHitboxes)
            {
                enemyHealthHitbox.gameObject.SetActive(true);
            }
        }
        public void DisableHealthHitboxes()
        {
            foreach (var enemyHealthHitbox in enemyHealthHitboxes)
            {
                enemyHealthHitbox.gameObject.SetActive(false);
            }
        }

        public void TakeDamage(AttackStatManager playerAttackStatManager, Weapon weapon, Transform attackerTransform, AudioClip weaponHitSfx)
        {
            if (!this.enabled)
            {
                return;
            }

            if (enemyBlockController != null && enemyBlockController.IsBlocking())
            {
                Instantiate(enemyBlockController.blockParticleEffect, enemyBlockController.shield.transform.position, Quaternion.identity);
                return;
            }

            if (this.currentHealth <= 0)
            {
                return;
            }

            BGMManager.instance.PlaySoundWithPitchVariation(
                weaponHitSfx,
                enemyCombatController.combatAudioSource
                );

            enemyCombatController.DisableAllWeaponHitboxes();

            float appliedDamage = weapon != null ? playerAttackStatManager.GetWeaponAttack(weapon) : playerAttackStatManager.GetCurrentPhysicalAttack();
            float physicalDamage = appliedDamage;

            if (enemyPostureController.IsStunned())
            {
                appliedDamage = appliedDamage * enemyPostureController.bonusMultiplier;
            }

            if (damageFloatingText != null)
            {
                damageFloatingText.Show(appliedDamage);
            }

            // Elemental Damage Bonus
            if (weapon != null)
            {
                if (weapon.fireAttack > 0)
                {
                    var elementalBonus = Mathf.Clamp(weapon.fireAttack + enemyElementalDamageController.fireDamageBonus, 0, Mathf.Infinity);
                    appliedDamage += elementalBonus;

                    enemyElementalDamageController.OnFireDamage(elementalBonus);
                }
                if (weapon.frostAttack > 0)
                {
                    var elementalBonus = Mathf.Clamp(weapon.frostAttack + enemyElementalDamageController.frostDamageBonus, 0, Mathf.Infinity);
                    appliedDamage += elementalBonus;

                    enemyElementalDamageController.OnFrostDamage(elementalBonus);
                }
            }

            this.currentHealth = Mathf.Clamp(currentHealth - appliedDamage, 0f, maxHealth);

            if (damageParticlePrefab != null)
            {
                Instantiate(damageParticlePrefab, transform.position, Quaternion.identity);
            }

            // Status Effects
            if (weapon != null && weapon.statusEffects != null && weapon.statusEffects.Length > 0)
            {
                foreach (var weaponStatusEffectPerHit in weapon.statusEffects)
                {
                    enemyStatusManager.InflictStatusEffect(weaponStatusEffectPerHit.statusEffect, weaponStatusEffectPerHit.amountPerHit);
                }
            }

            if (this.currentHealth <= 0)
            {

                Loot loot = enemy.GetComponent<Loot>();
                if (loot != null)
                {
                    StartCoroutine(GiveLoot(loot));
                }

                Die();
            }
            else
            {
                enemyPoiseController.IncreasePoiseDamage(weapon != null ? weapon.poiseDamageBonus : 1);
            }
        }

        IEnumerator GiveLoot(Loot loot)
        {
            yield return new WaitForSeconds(1f);

            Player.instance.currentGold += (int)enemy.goldReceived;

            // notificationManager.NotifyCoins(enemy.experienceGained);

            loot.GetLoot();
        }

        public void Die()
        {
            StartCoroutine(PlayDeathSfx());
            
            enemy.animator.SetTrigger(hashDying);

            StartCoroutine(DisengageLockOn());
        }

        protected IEnumerator PlayDeathSfx()
        {
            yield return new WaitForSeconds(0.1f);

            BGMManager.instance.PlaySoundWithPitchVariation(deathGruntSfx, enemyCombatController.combatAudioSource);

            yield return new WaitForSeconds(1f);

            healthBarSlider.gameObject.SetActive(false);

            enemyPostureController.postureBarSlider.gameObject.SetActive(false);
            enemyPostureController.stunnedParticle.gameObject.SetActive(false);
            enemyPostureController.enabled = false;

            this.enabled = false;
        }


        IEnumerator DisengageLockOn()
        {
            yield return new WaitForSeconds(1f);

            if (lockOnManager.isLockedOn
                && lockOnRef != null
                && lockOnRef.enabled
                && lockOnManager.nearestLockOnTarget != null
                && lockOnManager.nearestLockOnTarget == lockOnRef)
            {

                if (lockOnManager.rightLockTarget != null && lockOnManager.rightLockTarget != lockOnRef)
                {
                    lockOnManager.SwitchToRightTarget();
                }
                else if (lockOnManager.leftLockTarget != null && lockOnManager.leftLockTarget != lockOnRef)
                {
                    lockOnManager.SwitchToLeftTarget();
                }
                else
                {
                    lockOnManager.DisableLockOn();
                }
            }
        }

    }
}
