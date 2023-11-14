using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        CombatNotificationsController combatNotificationsController => GetComponentInParent<CombatNotificationsController>();
        SceneSettings sceneSettings;

        public GameObject shieldWoodImpactFx;
        public FloatingText damageFloatingText;

        [Header("Sounds")]
        public AudioClip deathGruntSfx;

        [Header("Elemental FX")]
        public ParticleSystem fireFx;
        public ParticleSystem frostFx;
        public ParticleSystem lightningFx;
        public ParticleSystem magicFx;

        Animator animator => GetComponentInParent<Animator>();

        public bool isInvencible = false;

        PlayerPoiseController playerPoiseController => GetComponentInParent<PlayerPoiseController>();

        [Header("Fart")]
        public bool fartOnHit = false;
        public DestroyableParticle farting;

        bool hasSurvivedDeath = false;

        [Header("Consumable Effects")]
        public bool isImmuneToFireDamage = false;

        [Header("Components")]
        public PlayerAchievementsManager playerAchievementsManager;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;

        private void Awake()
        {
            sceneSettings = FindObjectOfType<SceneSettings>(true);
        }

        public void Event_TakeDamage(float damage)
        {
            TakeEnvironmentalDamage(damage, 0, false, 0, WeaponElementType.None);
        }

        public void PlayFireFX()
        {
            if (fireFx == null)
            {
                return;
            }

            fireFx.gameObject.SetActive(false);
            fireFx.gameObject.SetActive(true);
            fireFx.Play();
        }

        public void PlayMagicFX()
        {
            if (magicFx == null)
            {
                return;
            }

            magicFx.gameObject.SetActive(false);
            magicFx.gameObject.SetActive(true);
            magicFx.Play();
        }

        public void PlayFrostFX()
        {
            if (frostFx == null)
            {
                return;
            }

            frostFx.gameObject.SetActive(false);
            frostFx.gameObject.SetActive(true);
            frostFx.Play();
        }

        public void PlayLightningFX()
        {
            if (lightningFx == null)
            {
                return;
            }

            lightningFx.gameObject.SetActive(false);
            lightningFx.gameObject.SetActive(true);
            lightningFx.Play();
        }

        public void TakeEnvironmentalDamage(float damage, int poiseDamage, bool ignoreDodging, int elementalDamage, WeaponElementType weaponElementType)
        {

            if (playerStatsDatabase.currentHealth <= 0)
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

            if (weaponElementType == WeaponElementType.Fire && elementalDamage > 0)
            {
                if (isImmuneToFireDamage)
                {
                    elementalDamage = 0;
                }
                else
                {
                    combatNotificationsController.ShowFireDamage(Mathf.RoundToInt(elementalDamage));

                    PlayFireFX();
                }
            }
            if (weaponElementType == WeaponElementType.Frost && elementalDamage > 0)
            {
                combatNotificationsController.ShowFrostDamage(Mathf.RoundToInt(elementalDamage));

                PlayFrostFX();
            }
            if (weaponElementType == WeaponElementType.Lightning && elementalDamage > 0)
            {
                combatNotificationsController.ShowLightningDamage(Mathf.RoundToInt(elementalDamage));

                PlayLightningFX();
            }
            if (weaponElementType == WeaponElementType.Magic && elementalDamage > 0)
            {
                combatNotificationsController.ShowMagicDamage(Mathf.RoundToInt(elementalDamage));

                PlayMagicFX();
            }
            healthStatManager.SubtractAmount(elementalDamage);


            if (damage > 0)
            {
                combatNotificationsController.ShowDamage(Mathf.RoundToInt(damage));
            }
            healthStatManager.SubtractAmount((int)
            damage);

            if (playerPoiseController.damageParticlePrefab != null)
            {
                Instantiate(playerPoiseController.damageParticlePrefab, transform.position, Quaternion.identity);
            }

            if (fartOnHit)
            {
                Instantiate(farting, transform.position, Quaternion.identity);
            }

            if (playerStatsDatabase.currentHealth <= 0)
            {
                Die();
            }
            else if (poiseDamage > 0)
            {
                playerPoiseController.IncreasePoiseDamage(poiseDamage);
            }
        }

        public void TakeDamage(
            float damage, Transform attackerTransform, AudioClip weaponDamageSfx, int poiseDamage, int elementalDamage, WeaponElementType weaponElementType)
        {

            if (fartOnHit)
            {
                Instantiate(farting, transform.position, Quaternion.identity);
            }

            if (isInvencible)
            {
                return;
            }

            if (playerStatsDatabase.currentHealth <= 0)
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
                if (equipmentDatabase.GetCurrentShield() != null)
                {
                    blockStaminaCost = equipmentDatabase.GetCurrentShield().blockStaminaCost;
                    defenseAbsorption = equipmentDatabase.GetCurrentShield().defenseAbsorption;
                }
                else if (equipmentDatabase.GetCurrentWeapon() != null)
                {
                    blockStaminaCost = equipmentDatabase.GetCurrentWeapon().blockStaminaCost;
                    defenseAbsorption = equipmentDatabase.GetCurrentWeapon().blockAbsorption;
                }

                float staminaCost = blockStaminaCost + enemy.enemyCombatController.bonusBlockStaminaCost;

                bool playerCanBlock = staminaStatManager.HasEnoughStaminaForAction(staminaCost);

                if (equipmentDatabase.GetCurrentShield() != null)
                {
                    Instantiate(equipmentDatabase.GetCurrentShield().blockFx, equipmentGraphicsHandler.shieldGraphic.transform);
                }
                else if (equipmentDatabase.GetCurrentWeapon() != null && equipmentGraphicsHandler.leftHand != null)
                {
                    if (equipmentDatabase.GetCurrentWeapon().blockFx != null)
                    {
                        Instantiate(equipmentDatabase.GetCurrentWeapon().blockFx, equipmentGraphicsHandler.leftHand.transform.position, Quaternion.identity);
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

                if (equipmentDatabase.GetCurrentShield() != null && equipmentDatabase.GetCurrentShield().damageDealtToEnemiesUponBlocking > 0)
                {
                    enemy.enemyHealthController.TakeEnvironmentalDamage(equipmentDatabase.GetCurrentShield().damageDealtToEnemiesUponBlocking);
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

            if (damage > 0)
            {
                combatNotificationsController.ShowDamage(Mathf.RoundToInt(damage));
            }

            if (weaponElementType == WeaponElementType.Fire && elementalDamage > 0)
            {
                if (isImmuneToFireDamage)
                {
                    elementalDamage = 0;
                }
                else
                {
                    combatNotificationsController.ShowFireDamage(Mathf.RoundToInt(elementalDamage));

                    PlayFireFX();
                }
            }
            if (weaponElementType == WeaponElementType.Frost && elementalDamage > 0)
            {
                combatNotificationsController.ShowFrostDamage(Mathf.RoundToInt(elementalDamage));

                PlayFrostFX();
            }
            if (weaponElementType == WeaponElementType.Lightning && elementalDamage > 0)
            {
                combatNotificationsController.ShowLightningDamage(Mathf.RoundToInt(elementalDamage));

                PlayLightningFX();
            }
            if (weaponElementType == WeaponElementType.Magic && elementalDamage > 0)
            {
                combatNotificationsController.ShowMagicDamage(Mathf.RoundToInt(elementalDamage));

                PlayMagicFX();
            }


            healthStatManager.SubtractAmount((int)damage + elementalDamage);

            if (weaponDamageSfx != null)
            {
                BGMManager.instance.PlaySound(weaponDamageSfx, playerCombatController.combatAudioSource);
            }

            if (playerStatsDatabase.currentHealth <= 0)
            {
                if (playerPoiseController.damageParticlePrefab != null)
                {
                    Instantiate(playerPoiseController.damageParticlePrefab, transform.position, Quaternion.identity);
                }

                Die();
            }
            else if (playerParryManager.IsBlocking() == false)
            {
                playerPoiseController.IncreasePoiseDamage(poiseDamage);
            }
        }

        public void Die()
        {
            if (hasSurvivedDeath == false && equipmentDatabase.accessories.FirstOrDefault(x => x != null && x.chanceToSurviveDeath) != null)
            {
                float chanceToSurvive = Random.Range(0, 100f);
                if (chanceToSurvive >= 50)
                {
                    healthStatManager.RestoreHealthPercentage(15);
                    hasSurvivedDeath = true;
                    FindAnyObjectByType<NotificationManager>(FindObjectsInactive.Include).ShowNotification(
                        GamePreferences.instance.IsEnglish()
                        ? "Crimson Ring has saved you from death"
                        : "O Anel Escarlate salvou-te da morte", null);
                    return;
                }
            }

            if (sceneSettings.isSceneTutorial)
            {
                sceneSettings.RestartTutorialFromCheckpoint(transform.position);
                return;
            }

            playerAchievementsManager.achievementForDyingFirstTime.AwardAchievement();


            playerStatsDatabase.currentHealth = 0;
            animator.SetBool("IsDodging", false);


            // Clear all negative statuses
            playerStatusManager.RemoveAllStatus();

            StartCoroutine(PlayDeathSfx());

            //playerComponentManager.DisableCharacterController();
            playerComponentManager.DisableComponents();

            animator.Play(hashDying);
            animator.SetBool("IsDying", true);
            animator.SetBool("IsCrouched", false);

            // Remove active consumables
            playerCombatController.GetComponent<PlayerConsumablesManager>().ClearAllConsumables();


            if (sceneSettings.isColliseum)
            {
                FindAnyObjectByType<RoundManager>(FindObjectsInactive.Include).CallEndTournament();
            }
            else
            {
                StartCoroutine(ShowGameOverScreen());
            }
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
