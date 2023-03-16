using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace AF
{
    public class EnemyHealthController : MonoBehaviour
    {

        [Header("Health")]
        public float currentHealth;
        public int maxHealthOverride = -1;

        [Header("UI Components")]
        public UnityEngine.UI.Slider healthBarSlider;
        public FloatingText damageFloatingText;
        
        [Header("On Killing Events")]
        public UnityEvent onEnemyDeath;
        public Collider[] collidersToDisableOnDeath;

        [Header("Elemental Damage")]
        public FloatingText elementalDamageFloatingText;

        [HideInInspector] public EnemyHealthHitbox[] enemyHealthHitboxes => GetComponentsInChildren<EnemyHealthHitbox>(true);

        // Components
        EnemyManager enemyManager => GetComponent<EnemyManager>();
        EnemyNegativeStatusController enemyNegativeStatusController => GetComponent<EnemyNegativeStatusController>();
        EnemyLoot enemyLoot => GetComponent<EnemyLoot>();
        EnemyBossController enemyBossController => GetComponent<EnemyBossController>();
        SceneSettings sceneSettings => FindObjectOfType<SceneSettings>(true);

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
        }


        void RestoreHealth()
        {
            this.currentHealth = GetMaxHealth();
        }

        public void ShowHUD()
        {
            healthBarSlider.transform.GetChild(0).gameObject.SetActive(true);
            healthBarSlider.transform.GetChild(1).gameObject.SetActive(true);
            healthBarSlider.transform.GetChild(2).gameObject.SetActive(true);
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
            }
            else
            {
                // Is Regular Enemy
                if (enemyBossController == null)
                {
                    if (healthBarSlider != null)
                    {
                        healthBarSlider.enabled = true;
                        healthBarSlider.maxValue = GetMaxHealth() * 0.01f;
                        healthBarSlider.value = currentHealth * 0.01f;
                    }
                }
                else // Is Boss
                {
                    var percentage = currentHealth * 100 / GetMaxHealth();
                    enemyBossController.UpdateBossHUDHealthPercentage(percentage);
                }
            }
        }

        public int GetMaxHealth()
        {
            return Player.instance.CalculateAIHealth(maxHealthOverride != -1 ? maxHealthOverride : enemyManager.enemy.baseHealth, enemyManager.currentLevel);
        }

        public void InitializeEnemyHUD()
        {
            if (healthBarSlider != null && enemyBossController == null)
            {
                healthBarSlider.gameObject.SetActive(true);
                healthBarSlider.maxValue = GetMaxHealth() * 0.01f;
                healthBarSlider.value = currentHealth * 0.01f;
            }
        }
        public void TakeEnvironmentalDamage(float damage)
        {
            if (this.currentHealth <= 0)
            {
                return;
            }

            this.currentHealth = Mathf.Clamp(currentHealth - damage, 0f, GetMaxHealth());


            if (damageFloatingText != null && damage != Mathf.Infinity)
            {
                damageFloatingText.Show(damage);
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

        public void TakeDamage(AttackStatManager playerAttackStatManager, Weapon weapon, Transform attackerTransform, AudioClip weaponHitSfx, float hitboxDamageBonus)
        {
            if (this.currentHealth <= 0)
            {
                return;
            }

            if (enemyManager.enemyBlockController != null && enemyManager.enemyBlockController.IsBlocking())
            {
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

            float appliedDamage = weapon != null ? playerAttackStatManager.GetWeaponAttack(weapon) : playerAttackStatManager.GetCurrentPhysicalAttack();
            appliedDamage += hitboxDamageBonus;

            if (enemyManager.enemyPostureController.IsStunned())
            {
                appliedDamage *= enemyManager.enemy.brokenPostureDamageMultiplier;
            }

            // Disable stunned stars on hit
            enemyManager.enemyPostureController.stunnedParticle.SetActive(false);

            // Elemental Damage Bonus
            if (weapon != null)
            {
                if (weapon.fireAttack > 0)
                {
                    var elementalBonus = Mathf.Clamp(weapon.fireAttack + enemyManager.enemy.fireDamageBonus, 0, Mathf.Infinity);
                    appliedDamage += elementalBonus;

                    OnFireDamage(elementalBonus);
                }
                if (weapon.frostAttack > 0)
                {
                    var elementalBonus = Mathf.Clamp(weapon.frostAttack + enemyManager.enemy.frostDamageBonus, 0, Mathf.Infinity);
                    appliedDamage += elementalBonus;

                    OnFrostDamage(elementalBonus);
                }
            }

            this.currentHealth = Mathf.Clamp(currentHealth - appliedDamage, 0f, GetMaxHealth());

            if (enemyManager.enemy.damagedParticle != null)
            {
                Instantiate(enemyManager.enemy.damagedParticle, transform.position, Quaternion.identity);
            }

            // Status Effects
            var bonusDamageToShow = 0f;
            if (weapon != null && weapon.statusEffects != null && weapon.statusEffects.Length > 0)
            {
                foreach (var weaponStatusEffectPerHit in weapon.statusEffects)
                {
                    var storedResult = enemyNegativeStatusController.InflictStatusEffect(
                        weaponStatusEffectPerHit.statusEffect, weaponStatusEffectPerHit.amountPerHit);

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

            if (enemyManager.enemyBlockController.IsBlocking())
            {
                return;
            }

            if (this.currentHealth <= 0)
            {
                return;
            }

            if (enemyManager.enemySleepController.isSleeping)
            {
                enemyManager.enemySleepController.WakeUp();
            }

            enemyManager.enemyWeaponController.DisableAllWeaponHitboxes();

            float physicalDamage = damage;

            if (enemyManager.enemyPostureController.IsStunned())
            {
                damage *= enemyManager.enemy.brokenPostureDamageMultiplier;
            }

            // Disable stunned stars on hit
            enemyManager.enemyPostureController.stunnedParticle.SetActive(false);


            this.currentHealth = Mathf.Clamp(currentHealth - damage, 0f, GetMaxHealth());

            if (enemyManager.enemy.damagedParticle != null)
            {
                Instantiate(enemyManager.enemy.damagedParticle, transform.position, Quaternion.identity);
            }

            // Status Effects
            var bonusDamageToShow = 0f;

            if (damageFloatingText != null)
            {
                damageFloatingText.Show(physicalDamage + bonusDamageToShow);
            }


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

            if (enemyBossController != null)
            {
                if (enemyBossController.fogWall != null)
                {
                    enemyBossController.fogWall.gameObject.SetActive(false);
                }

                enemyBossController.HideBossHud();
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

            yield return DisengageLockOn();

            // Update switch must be at the very last part of all the die flow
            if (enemyBossController != null && enemyBossController.bossSwitchEntry != null)
            {
                if (enemyBossController.refreshEventsUponSwitchActivation == false)
                {
                    SwitchManager.instance.UpdateSwitchWithoutRefreshingEvents(enemyBossController.bossSwitchEntry, true);
                }
                else
                {
                    SwitchManager.instance.UpdateSwitch(enemyBossController.bossSwitchEntry, true);
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
            yield return new WaitForSeconds(1f);

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
        }

        public void OnFireDamage(float appliedAmount)
        {
            elementalDamageFloatingText.gameObject.SetActive(true);
            elementalDamageFloatingText.GetComponent<TMPro.TextMeshPro>().color = sceneSettings.elementalFireTextColor;
            elementalDamageFloatingText.Show(appliedAmount);

            Instantiate(sceneSettings.elementalFireDamageFx, this.transform.position, Quaternion.identity);
        }

        public void OnFrostDamage(float appliedAmount)
        {
            elementalDamageFloatingText.gameObject.SetActive(true);
            elementalDamageFloatingText.GetComponent<TMPro.TextMeshPro>().color = sceneSettings.elementalFrostTextColor;
            elementalDamageFloatingText.Show(appliedAmount);

            Instantiate(sceneSettings.elementalFrostDamageFx, this.transform.position, Quaternion.identity);
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

    }
}
