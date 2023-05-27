using Mono.Cecil.Cil;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static AF.GamePreferences;

namespace AF
{
    [System.Serializable]
    public class WeaponTypeBonusTable
    {
        public WeaponAttackType weaponAttackType;
        [Range(1, 2.5f)] public float damageBonusMultiplier = 1;
    }
    [System.Serializable]
    public class NegativeWeaponTypeBonusTable
    {
        public WeaponAttackType weaponAttackType;
        [Range(0.1f, 1f)] public float negativeDamageBonusMultiplier = 1;
    }

    public class EnemyHealthController : MonoBehaviour
    {
        [Header("Health")]
        public float currentHealth;
        public int maxHealthOverride = -1;
        public UnityEvent onTakingDamage;

        [Header("Weapon Type Damage")]
        public WeaponTypeBonusTable[] weaponTypeBonusTable;
        public NegativeWeaponTypeBonusTable[] negativeWeaponTypeBonusTable;

        [Header("UI Components")]
        public UnityEngine.UI.Slider healthBarSlider;
        
        [Header("On Killing Events")]
        public UnityEvent onEnemyDeath;
        public UnityEvent onEnemyDeathAfter1Second;
        public Collider[] collidersToDisableOnDeath;

        [HideInInspector] public EnemyHealthHitbox[] enemyHealthHitboxes => GetComponentsInChildren<EnemyHealthHitbox>(true);

        // Components
        EnemyManager enemyManager => GetComponent<EnemyManager>();
        EnemyNegativeStatusController enemyNegativeStatusController => GetComponent<EnemyNegativeStatusController>();
        EnemyLoot enemyLoot => GetComponent<EnemyLoot>();
        EnemyBossController enemyBossController => GetComponent<EnemyBossController>();
        CombatNotificationsController combatNotificationsController => GetComponent<CombatNotificationsController>();
        SceneSettings sceneSettings;

        int amountToRestore = 0;

        public float scoreIncreaseRate = 10f;
        float ScoreIncrement = 0;
        bool onTakingDamageEventHasRun = false;

        public bool canTakeDamage = true;

        public string analyticsMessage;

        PlayerParryManager playerParryManager;

        private void Awake()
        {
             sceneSettings = FindObjectOfType<SceneSettings>(true);
             playerParryManager = FindObjectOfType<PlayerParryManager>(true);
        }

        private void Start()
        {
            if (enemyHealthHitboxes == null || enemyHealthHitboxes.Length <= 0)
            {
                Debug.LogError("Could not find any enemy health hitboxes on " + gameObject.name + " children. Please add one");
            }


            RestoreHealth();

            InitializeEnemyHUD();
        }

        private void Update()
        {
            UpdateHealthSlider();

            UpdateAmountToRestore();
        }


        void RestoreHealth()
        {
            this.currentHealth = GetMaxHealth();
        }

        public void ShowHUD()
        {
            if (enemyBossController == null)
            {
                healthBarSlider.transform.GetChild(0).gameObject.SetActive(true);
                healthBarSlider.transform.GetChild(1).gameObject.SetActive(true);
                healthBarSlider.transform.GetChild(2).gameObject.SetActive(true);
            }
        }

        public void HideHUD()
        {
            healthBarSlider.transform.GetChild(0).gameObject.SetActive(false);
            healthBarSlider.transform.GetChild(1).gameObject.SetActive(false);
            healthBarSlider.transform.GetChild(2).gameObject.SetActive(false);
        }

        void UpdateHealthSlider()
        {
            if (enemyManager.agent == false && healthBarSlider.isActiveAndEnabled)
            {
                healthBarSlider.gameObject.SetActive(false);
                return;
            }

            if (enemyBossController != null)
            {
                // Boss
                var percentage = currentHealth * 100 / GetMaxHealth();
                enemyBossController.UpdateBossHUDHealthPercentage(percentage);
                return;
            }

            if (healthBarSlider != null)
            {
                healthBarSlider.enabled = true;
                healthBarSlider.maxValue = GetMaxHealth() * 0.01f;
                healthBarSlider.value = currentHealth * 0.01f;
            }
        }

        public int GetMaxHealth()
        {
            return Player.instance.CalculateAIHealth(maxHealthOverride != -1 ? maxHealthOverride : enemyManager.enemy.baseHealth, enemyManager.currentLevel);
        }

        public void InitializeEnemyHUD()
        {
            if (healthBarSlider != null)
            {
                healthBarSlider.gameObject.SetActive(true);

                if (enemyBossController != null)
                {
                    healthBarSlider.enabled = false;
                    HideHUD();
                    return;
                }

                healthBarSlider.enabled = true;
                healthBarSlider.maxValue = GetMaxHealth() * 0.01f;
                healthBarSlider.value = currentHealth * 0.01f;
            }
        }

        public void TakeEnvironmentalDamage(float damage)
        {
            if (!canTakeDamage)
            {
                return;
            }

            if (this.currentHealth <= 0)
            {
                return;
            }

            this.currentHealth = Mathf.Clamp(currentHealth - damage, 0f, GetMaxHealth());


            if (damage != Mathf.Infinity && damage != 9999999)
            {
                combatNotificationsController.ShowDamage(damage);
            }

            if (enemyManager.enemy.damagedParticle != null)
            {
                Instantiate(enemyManager.enemy.damagedParticle, transform.position, Quaternion.identity);
            }

            if (this.currentHealth <= 0)
            {
                Die();
            }
        }

        public void RestoreHealthPercentage(int percentage)
        {
            float amount = GetMaxHealth() * percentage / 100;

            this.amountToRestore = (int)amount;
        }

        void UpdateAmountToRestore()
        {
            if (amountToRestore > 0)
            {
                if (this.currentHealth <= 0)
                {
                    amountToRestore = 0;
                    return;
                }

                ScoreIncrement += Time.deltaTime * scoreIncreaseRate;

                amountToRestore -= (int)ScoreIncrement;
                if (amountToRestore < 0) { amountToRestore = 0; }

                this.currentHealth += (int)ScoreIncrement;

                if (currentHealth > GetMaxHealth()) { currentHealth = GetMaxHealth(); }
            }
        }

        public void TakeDamage(AttackStatManager playerAttackStatManager, Weapon weapon, Vector3 collisionPoint, AudioClip weaponHitSfx, float hitboxDamageBonus)
        {
            if (!canTakeDamage)
            {
                return;
            }

            if (this.currentHealth <= 0)
            {
                return;
            }

            if (onTakingDamage != null && onTakingDamageEventHasRun == false)
            {
                onTakingDamage.Invoke();
                onTakingDamageEventHasRun = true;
            }

            if (enemyManager.enemyBlockController != null && Random.Range(0, 100) < enemyManager.enemyBlockController.parryWeight)
            {
                enemyManager.enemyBlockController.ActivateParry();
                return;
            }

            enemyManager.PushEnemy(weapon != null ? (int)weapon.pushForce : 1f);;

            if (enemyManager.enemyBlockController != null && enemyManager.enemyBlockController.IsBlocking())
            {
                enemyManager.enemyBlockController.HandleBlock(collisionPoint, weapon.blockHitAmount + (playerAttackStatManager.IsHeavyAttacking() ? 1 : 0));

                return;
            }

            if (enemyManager.enemySleepController != null && enemyManager.enemySleepController.isSleeping)
            {
                enemyManager.enemySleepController.WakeUp();
            }

            if (enemyBossController == null && enemyManager.shouldPlayBattleMusic)
            {
                BGMManager.instance.PlayBattleMusic();
            }

            BGMManager.instance.PlaySoundWithPitchVariation(
                weaponHitSfx,
                enemyManager.combatAudioSource
                );

            enemyManager.enemyWeaponController.DisableAllWeaponHitboxes();

            if (enemyLoot != null && weapon != null && weapon.amountOfGoldReceivedPerHit != 0)
            {
                enemyLoot.bonusGold += weapon.amountOfGoldReceivedPerHit;
            }

            float appliedDamage = weapon != null ? playerAttackStatManager.GetWeaponAttack(weapon) : playerAttackStatManager.GetCurrentPhysicalAttack();
            appliedDamage += hitboxDamageBonus;

            if (enemyManager.enemyPostureController.IsStunned())
            {
                appliedDamage *= enemyManager.enemy.brokenPostureDamageMultiplier;
            }

            // Disable stunned stars on hit
            enemyManager.enemyPostureController.stunnedParticle.SetActive(false);

            if (playerParryManager.IsWithinCounterAttackWindow())
            {
                appliedDamage *= playerParryManager.counterAttackMultiplier; // More attack power from counter attack
            }

            // Raw damage to display without elemental and status bonuses
            var displayedDamage = appliedDamage;

            // Elemental Damage Bonus
            if (weapon != null)
            {
                if (weapon.fireAttack > 0)
                {
                    var elementalBonus = Mathf.Clamp((int)(weapon.GetWeaponFireAttack() * enemyManager.enemy.fireDamageBonus), 0, Mathf.Infinity);
                    appliedDamage += elementalBonus;

                    combatNotificationsController.ShowFireDamage(elementalBonus);

                    Instantiate(weapon.elementImpactFx, collisionPoint, Quaternion.identity);
                }
                if (weapon.frostAttack > 0)
                {
                    var elementalBonus = Mathf.Clamp(weapon.GetWeaponFrostAttack() * enemyManager.enemy.frostDamageBonus, 0, Mathf.Infinity);
                    appliedDamage += elementalBonus;

                    combatNotificationsController.ShowFrostDamage(elementalBonus);

                    Instantiate(weapon.elementImpactFx, collisionPoint, Quaternion.identity);
                }
                if (weapon.lightningAttack > 0)
                {
                    var elementalBonus = Mathf.Clamp(weapon.GetWeaponLightningAttack() * enemyManager.enemy.lightningDamageBonus, 0, Mathf.Infinity);
                    appliedDamage += elementalBonus;

                    combatNotificationsController.ShowLightningDamage(elementalBonus);

                    //Instantiate(weapon.elementImpactFx, collisionPoint, Quaternion.identity);
                }
                if (weapon.magicAttack > 0)
                {
                    var elementalBonus = Mathf.Clamp(weapon.GetWeaponMagicAttack() * enemyManager.enemy.magicDamageBonus, 0, Mathf.Infinity);
                    appliedDamage += elementalBonus;

                    combatNotificationsController.ShowMagicDamage(elementalBonus);

                    //Instantiate(weapon.elementImpactFx, collisionPoint, Quaternion.identity);
                }

                if (weaponTypeBonusTable.Length > 0)
                {
                    var weaponTypeBonus = weaponTypeBonusTable.FirstOrDefault(entry => entry.weaponAttackType == weapon.weaponAttackType);
                    if (weaponTypeBonus != null)
                    {
                        appliedDamage = (int)(appliedDamage * weaponTypeBonus.damageBonusMultiplier);
                    }
                }
                if (negativeWeaponTypeBonusTable.Length > 0)
                {
                    var weaponTypeBonus = negativeWeaponTypeBonusTable.FirstOrDefault(entry => entry.weaponAttackType == weapon.weaponAttackType);
                    if (weaponTypeBonus != null)
                    {
                        appliedDamage = (int)(appliedDamage * weaponTypeBonus.negativeDamageBonusMultiplier);
                    }
                }

            }

            this.currentHealth = Mathf.Clamp(currentHealth - appliedDamage, 0f, GetMaxHealth());

            if (enemyManager.enemy.damagedParticle != null)
            {
                Instantiate(enemyManager.enemy.damagedParticle, collisionPoint, Quaternion.identity);
            }

            // Status Effects
            if (weapon != null && weapon.statusEffects != null && weapon.statusEffects.Length > 0)
            {
                foreach (var weaponStatusEffectPerHit in weapon.statusEffects)
                {
                    enemyNegativeStatusController.InflictStatusEffect(
                        weaponStatusEffectPerHit.statusEffect, weaponStatusEffectPerHit.amountPerHit);
                }
            }

            if (enemyManager.enemyPostureController.IsStunned())
            {
                combatNotificationsController.ShowCritical(displayedDamage);
            }
            else
            {
                combatNotificationsController.ShowDamage(displayedDamage);
            }

            if (this.currentHealth <= 0)
            {
                Die();
            }
            else
            {
                enemyManager.enemyPoiseController.IncreasePoiseDamage(weapon != null ? weapon.poiseDamageBonus : 1);
            }
        }

        public void TakeProjectileDamage(float damage, Projectile projectile)
        {
            Destroy(projectile.gameObject);

            if (enemyManager.enemyBlockController != null && enemyManager.enemyBlockController.IsBlocking())
            {
                enemyManager.enemyBlockController.HandleBlock(Vector3.zero, 1);
                return;
            }

            if (this.currentHealth <= 0)
            {
                return;
            }

            if (enemyManager.enemySleepController != null && enemyManager.enemySleepController.isSleeping)
            {
                enemyManager.enemySleepController.WakeUp();
            }

            if (enemyManager.enemyWeaponController != null)
            {
                enemyManager.enemyWeaponController.DisableAllWeaponHitboxes();
            }

            float physicalDamage = damage;

            if (enemyManager.enemyPostureController != null)
            {
                if (enemyManager.enemyPostureController.IsStunned())
                {
                    physicalDamage *= enemyManager.enemy.brokenPostureDamageMultiplier;
                }
                // Disable stunned stars on hit
                enemyManager.enemyPostureController.stunnedParticle.SetActive(false);
            }

            // Elemental Damage Bonus
            if (projectile != null)
            {
                if (projectile.projectileAttackElementType == WeaponElementType.Fire)
                {
                    var elementalBonus = Mathf.Clamp((int)(projectile.projectileDamage * enemyManager.enemy.fireDamageBonus), 0, Mathf.Infinity);
                    physicalDamage = elementalBonus;
                }

            }

            this.currentHealth = Mathf.Clamp(currentHealth - physicalDamage, 0f, GetMaxHealth());

            if (enemyManager.enemy.damagedParticle != null)
            {
                Instantiate(enemyManager.enemy.damagedParticle, transform.position, Quaternion.identity);
            }

            // Status Effects for bows
            combatNotificationsController.ShowDamage(physicalDamage);

            if (this.currentHealth <= 0)
            {
                Die();
            }
            else
            {
                enemyManager.enemyPoiseController.IncreasePoiseDamage(1);
            }

            // Not in combat
            if (enemyManager.enemyCombatController.IsInCombat() == false && enemyManager.enemyBehaviorController.becomeAgressiveOnAttacked)
            {
                enemyManager.enemyBehaviorController.TurnAgressive();
                enemyManager.animator.SetBool(enemyManager.hashChasing, true);
            }
        }

        public void Die()
        {
            if (string.IsNullOrEmpty(analyticsMessage) == false)
            {
                FindObjectOfType<Analytics>(true).TrackAnalyticsEvent(analyticsMessage);
            }

            StartCoroutine(DieFlow());
        }

        IEnumerator DieFlow()
        {
            enemyManager.animator.SetBool(enemyManager.hashDying, true);

            // Interrupt all buffs
            if (enemyManager.enemyBuffController != null)
            {
                enemyManager.enemyBuffController.InterruptAllBuffs();
            }

            if (enemyLoot != null)
            {
                if (enemyBossController != null)
                {
                    if (enemyBossController.AllBossesAreDead())
                    {
                        StartCoroutine(enemyLoot.GiveLoot());
                    }
                }
                else
                {
                    StartCoroutine(enemyLoot.GiveLoot());
                }
            }

            yield return PlayDeathSfx();

            yield return new WaitForSeconds(0.4f);
    
            yield return DisengageLockOn();

            if (enemyBossController != null && enemyBossController.AllBossesAreDead())
            {
                if (enemyBossController.fogWall != null)
                {
                    enemyBossController.fogWall.gameObject.SetActive(false);
                }

                enemyBossController.HideBossHud();

                if (enemyBossController.partner != null)
                {
                    if (enemyBossController.partner.fogWall != null)
                    {
                        enemyBossController.partner.fogWall.gameObject.SetActive(false);
                    }

                    enemyBossController.partner.HideBossHud();
                }
            }


            /*if (enemyManager.trackEnemyKill)
            {
                var playerActiveBuffs = "";
                foreach (var activeBuff in Player.instance.appliedConsumables)
                {
                    if (playerActiveBuffs != "")
                    {
                        playerActiveBuffs += ", ";
                    }

                    playerActiveBuffs += activeBuff.consumableEffect.displayName;
                }

                /*AnalyticsService.Instance.CustomData(isBoss ? "boss_killed" : "enemy_killed",
                        new Dictionary<string, object>()
                        {
                                { isBoss ? "boss" : "enemy", isBoss ? bossName : enemy.name + " (Lv. " + currentLevel + ") on map " + SceneManager.GetActiveScene().name },
                                { "player_level", FindObjectOfType<PlayerLevelManager>(true).GetCurrentLevel() },
                                { "player_vitality", Player.instance.vitality },
                                { "player_endurance", Player.instance.endurance },
                                { "player_strength", Player.instance.strength },
                                { "player_dexterity", Player.instance.dexterity },
                                { "player_weapon", Player.instance.equippedWeapon != null ? Player.instance.equippedWeapon.name : "Unarmed" },
                                { "player_shield", Player.instance.equippedShield != null ? Player.instance.equippedShield.name : "-" },
                                { "player_helmet", Player.instance.equippedHelmet != null ? Player.instance.equippedHelmet.name : "-" },
                                { "player_armor", Player.instance.equippedArmor != null ? Player.instance.equippedArmor.name : "-" },
                                { "player_gauntlets", Player.instance.equippedGauntlets != null ? Player.instance.equippedGauntlets.name : "-" },
                                { "player_legwear", Player.instance.equippedLegwear != null ? Player.instance.equippedLegwear.name : "-" },
                                { "player_acessory", Player.instance.equippedAccessory != null ? Player.instance.equippedAccessory.name : "-" },
                                { "player_buffs", playerActiveBuffs },
                        }
                    );*/
            

            // Notify companions that were attacking this enemy

            // Notify active playerManager companions
            CompanionManager[] companionManagers = FindObjectsOfType<CompanionManager>();
            foreach (var companion in companionManagers)
            {
                if (companion.currentEnemy == this)
                {
                    companion.StopCombat();
                }
            }

            foreach (var colliderToDisableOnDeath in collidersToDisableOnDeath)
            {
                colliderToDisableOnDeath.enabled = false;
            }

            enemyManager.enemyWeaponController.DisableAllWeaponHitboxes();

            enemyNegativeStatusController.ClearAllNegativeStatus();

            // Update switch must be at the very last part of all the die flow
            if (enemyBossController != null && enemyBossController.bossSwitchEntry != null && enemyBossController.AllBossesAreDead())
            {
                var targetBoss = enemyBossController.order == 1 ? enemyBossController : enemyBossController.partner;

                if (targetBoss != null)
                {
                    if (targetBoss.refreshEventsUponSwitchActivation == false)
                    {
                        SwitchManager.instance.UpdateSwitchWithoutRefreshingEvents(targetBoss.bossSwitchEntry, true);
                    }
                    else
                    {
                        SwitchManager.instance.UpdateSwitch(targetBoss.bossSwitchEntry, true, targetBoss.ignoredSwitchListener);
                    }

                    targetBoss.onBossBattleEnd.Invoke();
                }
            }
        }

        protected IEnumerator PlayDeathSfx()
        {
            yield return new WaitForSeconds(0.1f);

            BGMManager.instance.PlaySoundWithPitchVariation(enemyManager.enemy.isMale
                ? enemyManager.enemy.deathSfx
                : enemyManager.enemy.femaleDeathSfx,
                enemyManager.combatAudioSource);

            yield return new WaitForSeconds(1f);

            healthBarSlider.gameObject.SetActive(false);

            enemyManager.enemyPostureController.postureBarSlider.gameObject.SetActive(false);
            enemyManager.enemyPostureController.stunnedParticle.SetActive(false);

            BGMManager.instance.PlayMapMusicAfterKillingEnemy(enemyManager);
        }


        IEnumerator DisengageLockOn()
        {
            var lockOnManager = FindObjectOfType<LockOnManager>(true);

            var lockOnRef = GetComponentInChildren<LockOnRef>(true);

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

            yield return new WaitForSeconds(1f);

            if (onEnemyDeathAfter1Second != null)
            {
                if (enemyBossController != null)
                {

                    if (enemyBossController.AllBossesAreDead())
                    {
                        onEnemyDeathAfter1Second.Invoke();
                    }
                }
                else
                {
                    onEnemyDeathAfter1Second.Invoke();
                }
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

        public void AllowTakingDamage()
        {
            canTakeDamage = true;
        }
        public void DisableTakingDamage()
        {
            canTakeDamage = false;
        }

    }
}
