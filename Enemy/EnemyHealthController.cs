using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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

        [Header("For cases like pierce for skeletons, should absorb damage")]
        public NegativeWeaponTypeBonusTable[] negativeWeaponTypeBonusTable;

        [Header("UI Components")]
        public UnityEngine.UI.Slider healthBarSlider;

        [Header("On Killing Events")]
        public UnityEvent onEnemyDeath;
        public UnityEvent onEnemyDeathAfter1Second;
        public UnityEvent onEnemyDeathInColliseum;

        [Header("Holy Weapon Stuff")]
        public bool mustBeKilledWithHolyWeapon;
        public UnityEvent onKilledWithHolyWeapon;
        public UnityEvent onKilledWithOtherWeaponOtherThanHoly;

        [Header("FX")]
        public Color bloodColor;

        [HideInInspector] public List<EnemyHealthHitbox> enemyHealthHitboxes = new();

        public bool ignoreDefense = false;

        // Components
        EnemyManager enemyManager => GetComponent<EnemyManager>();
        EnemyNegativeStatusController enemyNegativeStatusController => GetComponent<EnemyNegativeStatusController>();
        EnemyLoot enemyLoot => GetComponent<EnemyLoot>();
        EnemyBossController enemyBossController => GetComponent<EnemyBossController>();
        CombatNotificationsController combatNotificationsController => GetComponent<CombatNotificationsController>();
        SceneSettings sceneSettings;
        EnemyPushableOnDeath enemyPushableOnDeath => GetComponent<EnemyPushableOnDeath>();

        LockOnManager lockOnManager;

        int amountToRestore = 0;

        public float scoreIncreaseRate = 10f;
        float ScoreIncrement = 0;
        bool onTakingDamageEventHasRun = false;

        public bool canTakeDamage = true;

        public string analyticsMessage;

        PlayerParryManager playerParryManager;
        PlayerCombatController playerCombatController;

        ParticlePoolManager particlePoolManager;

        float blockPostureDamageBonus = 1.25f;

        [Header("For enemies with multiple healthhitboxes")]
        public bool disableHealthhitboxesTemporarilyAfterHit = false;

        [Header("Reputation for Hell Skeletons")]
        public bool receiveReputationWhileReputationIsNegative = false;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;

        private void Awake()
        {
            sceneSettings = FindFirstObjectByType<SceneSettings>(FindObjectsInactive.Include);
            playerParryManager = FindFirstObjectByType<PlayerParryManager>(FindObjectsInactive.Include);
            playerCombatController = playerParryManager.GetComponent<PlayerCombatController>();
            lockOnManager = FindFirstObjectByType<LockOnManager>(FindObjectsInactive.Include);
            particlePoolManager = FindFirstObjectByType<ParticlePoolManager>(FindObjectsInactive.Include);

            foreach (var healthHitboxInChildren in GetComponentsInChildren<EnemyHealthHitbox>(true))
            {
                this.enemyHealthHitboxes.Add(healthHitboxInChildren);
            }

            foreach (var healthHitboxInThisObject in GetComponents<EnemyHealthHitbox>())
            {
                this.enemyHealthHitboxes.Add(healthHitboxInThisObject);
            }

        }

        private void Start()
        {
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
            var baseHealth = enemyManager.enemy.baseHealth;
            if (enemyManager.currentLevel > 1)
            {
                baseHealth = Player.instance.CalculateAIHealth(maxHealthOverride != -1 ? maxHealthOverride : enemyManager.enemy.baseHealth, enemyManager.currentLevel);
            }

            if (Player.instance.companions.Count == 1)
            {
                baseHealth = (int)(baseHealth * 1.025f);
            }
            else if (Player.instance.companions.Count == 2)
            {
                baseHealth = (int)(baseHealth * 1.1f);
            }
            else if (Player.instance.companions.Count > 2)
            {
                baseHealth = (int)(baseHealth * 1.2f);
            }

            if (enemyManager.healthReducingFactor > 1)
            {
                baseHealth = (int)(baseHealth / enemyManager.healthReducingFactor);
            }

            return baseHealth;
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

        public bool IsBelow50Percent()
        {
            var percentage = currentHealth * 100 / GetMaxHealth();
            return percentage <= 50;
        }

        public void UpdateMaxHealth()
        {
            if (enemyBossController != null)
            {
                return;
            }

            // If enemy is alive, update his current health to match the max health he now has
            if (currentHealth > 0)
            {
                currentHealth = GetMaxHealth();
            }

            healthBarSlider.maxValue = GetMaxHealth() * 0.01f;

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
                combatNotificationsController.ShowDamage(Mathf.RoundToInt(damage));
            }

            try
            {
                StartCoroutine(particlePoolManager.DropBloodOnEnemy(transform.position, bloodColor));

            }
            catch
            {

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
            if (enemyPushableOnDeath != null && enemyPushableOnDeath.isActivated)
            {
                enemyPushableOnDeath.Throw();
            }

            if (!canTakeDamage)
            {
                return;
            }

            if (this.currentHealth <= 0)
            {
                return;
            }

            if (enemyBossController == null && sceneSettings.isColliseum == false)
            {
                BGMManager.instance.PlayBattleMusic();
            }


            if (onTakingDamage != null && onTakingDamageEventHasRun == false)
            {
                onTakingDamage.Invoke();
                onTakingDamageEventHasRun = true;
            }

            if (enemyManager.enemyBlockController != null && enemyManager.enemyBlockController.CanParry())
            {
                enemyManager.enemyBlockController.ActivateParry();
                return;
            }

            enemyManager.PushEnemy(weapon != null ? (int)weapon.pushForce : 1f, ForceMode.Impulse); ;


            if (enemyManager.enemyBlockController != null && enemyManager.enemyBlockController.CanBlock())
            {
                int blockPostureDamage = playerCombatController.isHeavyAttacking || playerCombatController.IsJumpAttacking() || playerCombatController.IsStartingJumpAttack() ? 8 : 1;

                blockPostureDamage += weapon != null ? (int)weapon.pushForce : 1;

                float scaleFactor = Mathf.Sqrt(blockPostureDamage);

                int finalBlockPostureDamage = Mathf.RoundToInt(scaleFactor * 13.5f * blockPostureDamageBonus);

                enemyManager.enemyBlockController.HandleBlock(collisionPoint,
                    finalBlockPostureDamage);


                bool playerWeaponIgnoresShields = weapon == null ? false : weapon.ignoreShields;

                if (playerWeaponIgnoresShields == false)
                {
                    return;
                }
            }

            // CAN TAKE DAMAGE, LETS GO

            if (disableHealthhitboxesTemporarilyAfterHit)
            {
                DisableHealthHitboxes();
                StartCoroutine(ReenableHealthHitboxes());
            }

            if (enemyManager.enemySleepController != null && enemyManager.enemySleepController.isSleeping)
            {
                enemyManager.enemySleepController.WakeUp();
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

            if (weapon != null && weapon.doubleDamageDuringNightTime)
            {
                if (Player.instance.timeOfDay > 20 || Player.instance.timeOfDay < 5)
                {
                    appliedDamage *= 2;
                }
            }

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

            if (weapon != null)
            {

                if (weaponTypeBonusTable.Length > 0 && weapon != null)
                {
                    var weaponTypeBonus = weaponTypeBonusTable.FirstOrDefault(entry => entry.weaponAttackType == weapon.weaponAttackType);
                    if (weaponTypeBonus != null && weaponTypeBonus.damageBonusMultiplier > 0)
                    {
                        appliedDamage = appliedDamage * weaponTypeBonus.damageBonusMultiplier;
                    }
                }
                if (negativeWeaponTypeBonusTable.Length > 0 && weapon != null)
                {
                    var weaponTypeBonus = negativeWeaponTypeBonusTable.FirstOrDefault(entry => entry.weaponAttackType == weapon.weaponAttackType);
                    if (weaponTypeBonus != null && weaponTypeBonus.negativeDamageBonusMultiplier > 0)
                    {
                        appliedDamage = appliedDamage * weaponTypeBonus.negativeDamageBonusMultiplier;
                    }
                }
            }

            // Raw damage to display without elemental and status bonuses
            var displayedDamage = Mathf.RoundToInt(appliedDamage);

            // Elemental Damage Bonus
            if (weapon != null)
            {
                if (weapon.fireAttack > 0)
                {
                    var elementalBonus = Mathf.Clamp((int)(weapon.GetWeaponFireAttack() * enemyManager.enemy.fireDamageBonus), 0, Mathf.Infinity);
                    appliedDamage += elementalBonus;

                    combatNotificationsController.ShowFireDamage(Mathf.RoundToInt(elementalBonus));

                    if (weapon.elementImpactFx != null)
                    {
                        Instantiate(weapon.elementImpactFx, collisionPoint, Quaternion.identity);
                    }
                }
                if (weapon.frostAttack > 0)
                {
                    var elementalBonus = Mathf.Clamp(weapon.GetWeaponFrostAttack() * enemyManager.enemy.frostDamageBonus, 0, Mathf.Infinity);
                    appliedDamage += elementalBonus;

                    combatNotificationsController.ShowFrostDamage(Mathf.RoundToInt(elementalBonus));

                    if (weapon.elementImpactFx != null)
                    {
                        Instantiate(weapon.elementImpactFx, collisionPoint, Quaternion.identity);
                    }
                }
                if (weapon.lightningAttack > 0)
                {
                    var elementalBonus = Mathf.Clamp(weapon.GetWeaponLightningAttack() * enemyManager.enemy.lightningDamageBonus, 0, Mathf.Infinity);
                    appliedDamage += elementalBonus;

                    combatNotificationsController.ShowLightningDamage(Mathf.RoundToInt(elementalBonus));

                    //Instantiate(weapon.elementImpactFx, collisionPoint, Quaternion.identity);
                }
                if (weapon.magicAttack > 0)
                {
                    var elementalBonus = Mathf.Clamp(weapon.GetWeaponMagicAttack() * enemyManager.enemy.magicDamageBonus, 0, Mathf.Infinity);
                    appliedDamage += elementalBonus;

                    combatNotificationsController.ShowMagicDamage(Mathf.RoundToInt(elementalBonus));

                    if (weapon.elementImpactFx != null)
                    {
                        Instantiate(weapon.elementImpactFx, collisionPoint, Quaternion.identity);
                    }
                }


            }

            this.currentHealth = Mathf.Clamp(currentHealth - Mathf.RoundToInt(appliedDamage), 0f, GetMaxHealth());

            /*if (enemyManager.enemy.damagedParticle != null)
            {
                Instantiate(enemyManager.enemy.damagedParticle, collisionPoint, Quaternion.identity);
            }*/

            StartCoroutine(particlePoolManager.DropBloodOnEnemy(collisionPoint, bloodColor));


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
                return;
            }


            bool isHeavyAttacking = playerCombatController.isHeavyAttacking || playerCombatController.IsJumpAttacking() || playerCombatController.IsStartingJumpAttack();

            if (enemyManager.enemyPostureController != null)
            {
                int weaponPostureDamage = isHeavyAttacking ? 8 : 1;

                weaponPostureDamage += weapon != null ? (int)weapon.pushForce : 1;

                float scaleFactor = Mathf.Sqrt(weaponPostureDamage);
                int finalWeaponPostureDamage = Mathf.RoundToInt(scaleFactor * 12f);

                var hasBrokePosture = enemyManager.enemyPostureController.TakePostureDamage(finalWeaponPostureDamage);

                if (hasBrokePosture)
                {
                    return;
                }
            }

            enemyManager.enemyPoiseController.IncreasePoiseDamage(weapon != null ? (weapon.poiseDamageBonus * (isHeavyAttacking ? 2 : 1)) : 1);
        }

        IEnumerator ReenableHealthHitboxes()
        {
            yield return new WaitForSeconds(0.1f);
            EnableHealthHitboxes();
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

                    // Only allow 1 critical damage
                    enemyManager.animator.SetBool(enemyManager.hashIsStunned, false);
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

            StartCoroutine(particlePoolManager.DropBloodOnEnemy(transform.position, bloodColor));

            // Status Effects for bows
            combatNotificationsController.ShowDamage(Mathf.RoundToInt(damage));

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
            enemyManager.characterController.excludeLayers = LayerMask.GetMask("TempCast");
            enemyManager.agent.enabled = false;

            Player.instance.lastEnemyKilled = enemyManager.enemy;

            onEnemyDeath?.Invoke();

            if (receiveReputationWhileReputationIsNegative && Player.instance.equippedWeapon != null && Player.instance.equippedWeapon.isHolyWeapon)
            {
                if (playerStatsDatabase.GetCurrentReputation() < 0)
                {
                    FindAnyObjectByType<NotificationManager>(FindObjectsInactive.Include).IncreaseReputation(1);
                }
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
                StartCoroutine(enemyLoot.GiveLoot());
            }

            yield return PlayDeathSfx();

            yield return new WaitForSeconds(0.4f);

            yield return DisengageLockOn();

            if (mustBeKilledWithHolyWeapon)
            {
                if (Player.instance.equippedWeapon != null && Player.instance.equippedWeapon.isHolyWeapon)
                {
                    onKilledWithHolyWeapon.Invoke();
                }
                else
                {
                    onKilledWithOtherWeaponOtherThanHoly.Invoke();
                }
            }

            BGMManager.instance.PlayMapMusicAfterKillingEnemy(enemyManager);

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

                    // Reputation Lost
                    if (enemyBossController.reputationLostForKillingBoss > 0)
                    {
                        FindAnyObjectByType<NotificationManager>(FindObjectsInactive.Include).DecreaseReputation(enemyBossController.reputationLostForKillingBoss);
                    }

                    // Save game
                    // SaveSystem.instance.SaveGameData();//SceneManager.GetActiveScene().name);
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


            if (enemyPushableOnDeath != null)
            {
                enemyPushableOnDeath.Activate();
            }
        }


        IEnumerator DisengageLockOn()
        {
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

            if (onEnemyDeathInColliseum != null)
            {
                onEnemyDeathInColliseum.Invoke();
            }
        }

        public void EnableHealthHitboxes()
        {
            foreach (var enemyHealthHitbox in enemyHealthHitboxes)
            {
                enemyHealthHitbox.enabled = true;
            }
        }
        public void DisableHealthHitboxes()
        {
            foreach (var enemyHealthHitbox in enemyHealthHitboxes)
            {
                enemyHealthHitbox.enabled = false;
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
