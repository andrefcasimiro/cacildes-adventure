using System.Collections;
using UnityEngine;

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

        public GameObject shieldWoodImpactFx;
        public FloatingText damageFloatingText;

        [Header("Sounds")]
        public AudioClip deathGruntSfx;

        Animator animator => GetComponentInParent<Animator>();

        PlayerPoiseController playerPoiseController => GetComponentInParent<PlayerPoiseController>();

        public void TakeEnvironmentalDamage(float damage)
        {

            if (Player.instance.currentHealth <= 0)
            {
                return;
            }


            if (playerPoiseController.isStunned)
            {
                return;
            }


            if (dodgeController.IsDodging())
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

                Die();
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

            Enemy enemy = attackerTransform.GetComponent<Enemy>();
            EnemyCombatController enemyCombatController = enemy != null ? enemy.GetComponent<EnemyCombatController>() : null;

            if (
               playerParryManager.IsBlocking()
                // And enemy is facing weapon
                && Vector3.Angle(transform.forward * -1, attackerTransform.forward) <= 90f
                && enemyCombatController != null
                )
            {
                float blockStaminaCost = 0;
                float defenseAbsorption = 0;
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

                float staminaCost = blockStaminaCost + enemyCombatController.bonusBlockStaminaCost;

                bool playerCanBlock = staminaStatManager.HasEnoughStaminaForAction(staminaCost);

                if (Player.instance.equippedShield != null)
                {
                    Instantiate(Player.instance.equippedShield.blockFx, equipmentGraphicsHandler.shieldGraphic.transform);
                }
                else if (Player.instance.equippedWeapon != null && equipmentGraphicsHandler.leftHand != null)
                {
                    Instantiate(Player.instance.equippedWeapon.blockFx, equipmentGraphicsHandler.leftHand.transform.position, Quaternion.identity);
                }

                staminaStatManager.DecreaseStamina(staminaCost);

                if (playerCanBlock)
                {
                    damage = damage - ((int)(damage * defenseAbsorption / 100));
                    playerCombatController.GetComponent<Animator>().Play("Blocking Hit");
                }
                else
                {
                    // Can not block attack, stop blocking
                    playerCombatController.GetComponent<Animator>().SetBool(playerParryManager.hashIsBlocking, false);
                }
            }

            if (dodgeController.IsDodging())
            {
                return;
            }

            if (enemyCombatController != null && enemyCombatController.weaponStatusEffect != null)
            {
                playerStatusManager.InflictStatusEffect(enemyCombatController.weaponStatusEffect, enemyCombatController.statusEffectAmount, false);
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
            StartCoroutine(PlayDeathSfx());

            playerComponentManager.DisableCharacterController();
            playerComponentManager.DisableComponents();

            animator.Play(hashDying);

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

            FindObjectOfType<UIDocumentGameOver>(true).gameObject.SetActive(true);
        }
    }

}
