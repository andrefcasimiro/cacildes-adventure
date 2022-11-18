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

            GameObject shield = null;
            Enemy enemy = attackerTransform.GetComponent<Enemy>();
            EnemyCombatController enemyCombatController = enemy != null ? enemy.GetComponent<EnemyCombatController>() : null;

            if (
                // Is has shield
                shield != null
                // And shield is visible
                && shield.gameObject.activeSelf
                // And enemy is facing weapon
                && Vector3.Angle(transform.forward * -1, attackerTransform.forward) <= 90f
                && enemyCombatController != null
                )
            {
                float staminaCost = Player.instance.equippedShield.blockStaminaCost + enemyCombatController.bonusBlockStaminaCost;
                bool playerWontTakeDamage = staminaStatManager.HasEnoughStaminaForAction(staminaCost);

                Instantiate(shieldWoodImpactFx, shield.gameObject.transform);
                staminaStatManager.DecreaseStamina(staminaCost);

                if (playerWontTakeDamage)
                {
                    return;
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
            else
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
