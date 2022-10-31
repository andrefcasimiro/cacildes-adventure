using System.Collections;
using UnityEngine;

namespace AF
{
    public class PlayerHealthbox : MonoBehaviour
    {
        public readonly int hashTakingDamage = Animator.StringToHash("TakingDamage");
        public readonly int hashDying = Animator.StringToHash("Dying");
        public readonly int hashIsDead = Animator.StringToHash("Dead");

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

        [Header("Visuals")]
        public DestroyableParticle damageParticlePrefab;

        [Header("Sounds")]
        public AudioClip damageSfx;
        public AudioClip deathGruntSfx;

        Animator animator => GetComponentInParent<Animator>();

        public void TakeDamage(float damage, Transform attackerTransform, AudioClip weaponSwingSfx)
        {
            if (Player.instance.currentHealth <= 0)
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

            if (damageParticlePrefab != null)
            {
                Instantiate(damageParticlePrefab, transform.position, Quaternion.identity);
            }

            if (weaponSwingSfx != null)
            {
                BGMManager.instance.PlaySound(weaponSwingSfx, playerCombatController.combatAudioSource);
            }

            StartCoroutine(PlayHurtSfx());

            if (Player.instance.currentHealth <= 0)
            {
                Die();

                StartCoroutine(ShowGameOverScreen());
            }
            else
            {

                if (climbController.climbState == ClimbController.ClimbState.NONE)
                {
                    animator.SetTrigger(hashTakingDamage);
                }
            }
        }
        public void Die()
        {
            StartCoroutine(PlayDeathSfx());

            playerComponentManager.DisableCharacterController();
            playerComponentManager.DisableComponents();

            animator.SetTrigger(hashDying);
        }
        
        protected IEnumerator PlayHurtSfx()
        {
            yield return new WaitForSeconds(0.1f);
            BGMManager.instance.PlaySound(damageSfx, playerCombatController.combatAudioSource);
        }

        protected IEnumerator PlayDeathSfx()
        {
            yield return new WaitForSeconds(0.1f);
            BGMManager.instance.PlaySound(deathGruntSfx, playerCombatController.combatAudioSource);
        }
        
        IEnumerator ShowGameOverScreen()
        {
            yield return new WaitForSeconds(2f);
            /*UIDocumentGameOverScreen uIDocumentGameOverScreen = FindObjectOfType<UIDocumentGameOverScreen>(true);
            uIDocumentGameOverScreen.ShowGameOverScreen();*/
        }
    }

}
