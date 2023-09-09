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

        private void Awake()
        {
             sceneSettings = FindObjectOfType<SceneSettings>(true);
        }

        public void Event_TakeDamage(float damage)
        {
            TakeEnvironmentalDamage(damage, 0, false, WeaponElementType.None);
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

        public void TakeEnvironmentalDamage(float damage, int poiseDamage, bool ignoreDodging, WeaponElementType weaponElementType)
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



            if (weaponElementType == WeaponElementType.Fire)
            {
                PlayFireFX();
            }
            if (weaponElementType == WeaponElementType.Frost)
            {
                PlayFrostFX();
            }
            if (weaponElementType == WeaponElementType.Lightning)
            {
                PlayLightningFX();
            }
            if (weaponElementType == WeaponElementType.Magic)
            {
                PlayMagicFX();
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

            if (fartOnHit)
            {
                Instantiate(farting, transform.position, Quaternion.identity);
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

        public void TakeDamage(float damage, Transform attackerTransform, AudioClip weaponDamageSfx, int poiseDamage, WeaponElementType weaponElementType)
        {

            if (fartOnHit)
            {
                Instantiate(farting, transform.position, Quaternion.identity);
            }

            if (isInvencible)
            {
                return;
            }

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

                if (Player.instance.equippedShield != null && Player.instance.equippedShield.damageDealtToEnemiesUponBlocking > 0)
                {
                    enemy.enemyHealthController.TakeEnvironmentalDamage(Player.instance.equippedShield.damageDealtToEnemiesUponBlocking);
                }
            }

            if (dodgeController.hasIframes)
            {
                return;
            }

            if (weaponElementType == WeaponElementType.Fire)
            {
                PlayFireFX();
            }
            if (weaponElementType == WeaponElementType.Frost)
            {
                PlayFrostFX();
            }
            if (weaponElementType == WeaponElementType.Lightning)
            {
                PlayLightningFX();
            }
            if (weaponElementType == WeaponElementType.Magic)
            {
                PlayMagicFX();
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

            animator.SetBool("IsCrouched", false);
            animator.SetBool("IsDodging", false);

            StartCoroutine(PlayDeathSfx());

            //playerComponentManager.DisableCharacterController();
            playerComponentManager.DisableComponents();

            animator.Play(hashDying);
            animator.SetBool("IsDying", true);

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
