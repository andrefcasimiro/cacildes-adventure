using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF
{
    public class PlayerHealthbox : MonoBehaviour
    {
        public readonly int hashDying = Animator.StringToHash("Dying");

        // References
        PlayerCombatController playerCombatController => GetComponentInParent<PlayerCombatController>();
        DodgeController dodgeController => GetComponentInParent<DodgeController>();
        StaminaStatManager staminaStatManager => GetComponentInParent<StaminaStatManager>();
        PlayerStatusManager playerStatusManager => GetComponentInParent<PlayerStatusManager>();
        HealthStatManager healthStatManager => GetComponentInParent<HealthStatManager>();
        ClimbController climbController => GetComponentInParent<ClimbController>();
        PlayerComponentManager playerComponentManager => GetComponentInParent<PlayerComponentManager>();
        PlayerParryManager playerParryManager => GetComponentInParent<PlayerParryManager>();
        EquipmentGraphicsHandler equipmentGraphicsHandler => GetComponentInParent<EquipmentGraphicsHandler>();

        SceneSettings sceneSettings => FindObjectOfType<SceneSettings>(true);

        public GameObject shieldWoodImpactFx;
        public FloatingText damageFloatingText;

        [Header("Sounds")]
        public AudioClip deathGruntSfx;

        Animator animator => GetComponentInParent<Animator>();

        PlayerPoiseController playerPoiseController => GetComponentInParent<PlayerPoiseController>();

        public void TakeEnvironmentalDamage(float damage, int poiseDamage, bool ignoreDodging)
        {

            if (Player.instance.currentHealth <= 0)
            {
                return;
            }


            if (playerPoiseController.isStunned)
            {
                return;
            }


            if (dodgeController.hasIframes && ignoreDodging == false)
            {
                return;
            }


            healthStatManager.SubtractAmount(damage);

            if (damageFloatingText != null)
            {
                damageFloatingText.Show(damage);
            }

            if (playerPoiseController.damageParticlePrefab != null)
            {
                Instantiate(playerPoiseController.damageParticlePrefab, transform.position, Quaternion.identity);
            }

            if (Player.instance.currentHealth <= 0)
            {
                // Track player death
                /*AnalyticsService.Instance.CustomData("player_killed_by_environment",
                new Dictionary<string, object>()
                {
                                            { "killed_at", SceneManager.GetActiveScene().name },
                });*/

                Die();
            }
            else if (poiseDamage > 0)
            {
                playerPoiseController.IncreasePoiseDamage(poiseDamage);
            }
        }

        public void TakeDamage(float damage, Transform attackerTransform, AudioClip weaponDamageSfx, int poiseDamage)
        {
            if (Player.instance.currentHealth <= 0)
            {
                return;
            }

            if (playerPoiseController.isStunned)
            {
                return;
            }

            EnemyManager enemy = attackerTransform.GetComponent<EnemyManager>();

            if (
               playerParryManager.IsBlocking()
                // And enemy is facing weapon
                && Vector3.Angle(transform.forward * -1, attackerTransform.forward) <= 90f
                && enemy != null
                )
            {
                float blockStaminaCost = 0;
                float defenseAbsorption = playerParryManager.unarmedDefenseAbsorption; // Unarmed default
                if (Player.instance.equippedShield != null)
                {
                    blockStaminaCost = Player.instance.equippedShield.blockStaminaCost;
                    defenseAbsorption = Player.instance.equippedShield.defenseAbsorption;
                }
                else if (Player.instance.equippedWeapon != null)
                {
                    blockStaminaCost = Player.instance.equippedWeapon.blockStaminaCost;
                    defenseAbsorption = Player.instance.equippedWeapon.blockAbsorption;
                }

                float staminaCost = blockStaminaCost + enemy.enemyCombatController.bonusBlockStaminaCost;

                bool playerCanBlock = staminaStatManager.HasEnoughStaminaForAction(staminaCost);

                if (Player.instance.equippedShield != null)
                {
                    Instantiate(Player.instance.equippedShield.blockFx, equipmentGraphicsHandler.shieldGraphic.transform);
                }
                else if (Player.instance.equippedWeapon != null && equipmentGraphicsHandler.leftHand != null)
                {
                    if (Player.instance.equippedWeapon.blockFx != null)
                    {
                        Instantiate(Player.instance.equippedWeapon.blockFx, equipmentGraphicsHandler.leftHand.transform.position, Quaternion.identity);
                    }
                }

                staminaStatManager.DecreaseStamina(staminaCost);

                if (playerCanBlock)
                {
                    damage = damage - ((int)(damage * defenseAbsorption / 100));
                    playerCombatController.GetComponent<Animator>().Play("Block Hit");
                }
                else
                {
                    // Can not block attack, stop blocking
                    playerCombatController.GetComponent<Animator>().SetBool(playerParryManager.hashIsBlocking, false);
                }
            }

            if (dodgeController.hasIframes)
            {
                return;
            }

            if (enemy != null)
            {
                if (enemy.enemyCombatController.weaponStatusEffect != null)
                {
                    playerStatusManager.InflictStatusEffect(enemy.enemyCombatController.weaponStatusEffect, enemy.enemyCombatController.statusEffectAmount, false);
                }
            }

            healthStatManager.SubtractAmount(damage);

            if (damageFloatingText != null)
            {
                damageFloatingText.Show(damage);
            }

            if (weaponDamageSfx != null)
            {
                BGMManager.instance.PlaySound(weaponDamageSfx, playerCombatController.combatAudioSource);
            }

            if (Player.instance.currentHealth <= 0)
            {
                if (playerPoiseController.damageParticlePrefab != null)
                {
                    Instantiate(playerPoiseController.damageParticlePrefab, transform.position, Quaternion.identity);
                }

                // Track player death
                /*AnalyticsService.Instance.CustomData("player_killed_by_enemy",
                new Dictionary<string, object>()
                {
                                            { "killed_by", enemy.name + " (Lv. " + enemy.currentLevel + ") on map " + SceneManager.GetActiveScene().name },
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
                }
    );*/

                Die();
            }
            else if (playerParryManager.IsBlocking() == false)
            {
                playerPoiseController.IncreasePoiseDamage(poiseDamage);
            }
        }
        public void Die()
        {
            if (sceneSettings.isSceneTutorial)
            {
                sceneSettings.RestartTutorialFromCheckpoint(transform.position);
                return;
            }

            StartCoroutine(PlayDeathSfx());

            playerComponentManager.DisableCharacterController();
            playerComponentManager.DisableComponents();

            animator.Play(hashDying);

            // Remove active consumables
            playerCombatController.GetComponent<PlayerConsumablesManager>().ClearAllConsumables();


            StartCoroutine(ShowGameOverScreen());
        }
        
        protected IEnumerator PlayDeathSfx()
        {
            yield return new WaitForSeconds(0.1f);
            BGMManager.instance.PlaySound(deathGruntSfx, playerCombatController.combatAudioSource);
        }
        
        IEnumerator ShowGameOverScreen()
        {
            yield return new WaitForSeconds(2f);

            FindObjectOfType<LockOnManager>(true).DisableLockOn();
            FindObjectOfType<UIDocumentGameOver>(true).gameObject.SetActive(true);
        }
    }

}
