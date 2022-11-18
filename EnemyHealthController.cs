using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.UIElements;

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
        public UnityEngine.UI.Slider healthBarSlider;
        public FloatingText damageFloatingText;
        public UnityEngine.UIElements.UIDocument bossHud;
        public UnityEngine.UIElements.IMGUIContainer bossFillBar;

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
        EnemySleepController enemySleepController => GetComponent<EnemySleepController>();

        private EnemyHealthHitbox[] enemyHealthHitboxes;

        LockOnManager lockOnManager;
        LockOnRef lockOnRef;

        GameObject player;

        bool isDisabled = false;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        private void Start()
        {
            player = GameObject.FindWithTag("Player");

            lockOnManager = FindObjectOfType<LockOnManager>(true);
            lockOnRef = GetComponentInChildren<LockOnRef>(true);


            this.enemyHealthHitboxes = GetComponentsInChildren<EnemyHealthHitbox>(true);

            if (healthBarSlider != null && enemy.isBoss == false)
            {
                healthBarSlider.maxValue = maxHealth * 0.01f;
                healthBarSlider.value = currentHealth * 0.01f;
            }

            if (enemy.isBoss)
            {
                bossHud = GetComponent<UIDocument>();
                bossFillBar = bossHud.rootVisualElement.Q<IMGUIContainer>("hp-bar");
                bossHud.rootVisualElement.Q<Label>("boss-name").text = enemy.bossName;

                HideBossHud();
            }

        }

        private void OnEnable()
        {
            if (currentHealth <= 0)
            {
                this.gameObject.SetActive(false);
            }
        }

        private void Update()
        {   
            if (isDisabled)
            {
                return;
            }

            if (Vector3.Distance(player.transform.position, this.transform.position) > 10)
            {
                enemyPostureController.postureBarSlider.gameObject.SetActive(false);
             
                if (enemy.isBoss == false)
                {
                    healthBarSlider.gameObject.SetActive(false);
                }
                else if (enemy.InCombatWithPlayer() == false || currentHealth < 0)
                {
                    bossHud.rootVisualElement.style.display = DisplayStyle.None;
                }
            }
            else if (currentHealth > 0)
            {
                if (enemy.isBoss == false) {
                    healthBarSlider.gameObject.SetActive(true);
                }
            }

            if (healthBarSlider != null && enemy.isBoss == false)
            {
                healthBarSlider.value = currentHealth * 0.01f;
            }
            
            if (enemy.isBoss)
            {
                var percentage = currentHealth * 100 / maxHealth;
                bossFillBar.style.width = new Length(percentage, LengthUnit.Percent);
            }
        }

        public void ShowBossHud()
        {
            healthBarSlider.transform.GetChild(0).gameObject.SetActive(false);
            healthBarSlider.transform.GetChild(1).gameObject.SetActive(false);
            healthBarSlider.transform.GetChild(2).gameObject.SetActive(false);
            bossHud.rootVisualElement.style.display = DisplayStyle.Flex;
        }

        public void HideBossHud()
        {
            healthBarSlider.transform.GetChild(0).gameObject.SetActive(false);
            healthBarSlider.transform.GetChild(1).gameObject.SetActive(false);
            healthBarSlider.transform.GetChild(2).gameObject.SetActive(false);
            bossHud.rootVisualElement.style.display = DisplayStyle.None;
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

        public void TakeEnvironmentalDamage(float damage)
        {
            if (this.currentHealth <= 0)
            {
                return;
            }

            this.currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealth);


            if (damageParticlePrefab != null)
            {
                Instantiate(damageParticlePrefab, transform.position, Quaternion.identity);
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
        }

        public void TakeDamage(AttackStatManager playerAttackStatManager, Weapon weapon, Transform attackerTransform, AudioClip weaponHitSfx)
        {
            if (isDisabled)
            {
                return;
            }

            if (enemyBlockController != null && enemyBlockController.IsBlocking())
            {
                return;
            }

            if (this.currentHealth <= 0)
            {
                return;
            }

            if (enemySleepController != null && enemySleepController.isSleeping)
            {
                enemySleepController.WakeUp();
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
            var bonusDamageToShow = 0f;
            if (weapon != null && weapon.statusEffects != null && weapon.statusEffects.Length > 0)
            {
                foreach (var weaponStatusEffectPerHit in weapon.statusEffects)
                {
                    var storedResult = enemyStatusManager.InflictStatusEffect(weaponStatusEffectPerHit.statusEffect, weaponStatusEffectPerHit.amountPerHit);
                    if (storedResult > 0)
                    {
                        bonusDamageToShow += storedResult;
                    }
                }
            }

            if (damageFloatingText != null)
            {
                damageFloatingText.Show(appliedDamage + bonusDamageToShow);
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

            if (enemy.fogWall != null)
            {
                enemy.fogWall.gameObject.SetActive(false);
            }

            if (enemy.isBoss)
            {
                HideBossHud();

                SwitchManager.instance.UpdateSwitchWithoutRefreshingEvents(enemy.bossSwitchUuid, true);
            }

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

            isDisabled = true;

            // this.enabled = false;
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
