using System.Collections;
using UnityEngine;

namespace AF
{
    [RequireComponent(typeof(Player))]
    [RequireComponent(typeof(Player))]
    public class PlayerHealthbox : Healthbox
    {
        // References
        Player player => GetComponent<Player>();


        public GameObject shieldWoodImpactFx;
        public FloatingText damageFloatingText;

        public override void TakeDamage(float damage, Transform attackerTransform, AudioClip weaponSwingSfx)
        {
            ShieldInstance shield = this.combatable.GetShieldInstance();

            if (
                // Is has shield
                shield != null
                // And shield is visible
                && shield.gameObject.activeSelf
                // And enemy is facing weapon
                && Vector3.Angle(transform.forward * -1, attackerTransform.forward) <= 90f
                )
            {
                float staminaCost = PlayerInventoryManager.instance.currentShield.blockStaminaCost;
                bool playerWontTakeDamage = PlayerStatsManager.instance.HasEnoughStaminaForAction(staminaCost);

                Instantiate(shieldWoodImpactFx, shield.gameObject.transform);
                player.animator.Play(player.hashBlockingHit);
                PlayerStatsManager.instance.DecreaseStamina(staminaCost);

                if (playerWontTakeDamage)
                {
                    return;
                }
            }

            if (player.IsDodging())
            {
                return;
            }

            combatable.SetCurrentHealth(combatable.GetCurrentHealth() - damage);

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
                Utils.PlaySfx(combatAudioSource, weaponSwingSfx);
            }

            StartCoroutine(PlayHurtSfx());

            if (combatable.GetCurrentHealth() <= 0)
            {
                UIDocumentGameOverScreen uIDocumentGameOverScreen = FindObjectOfType<UIDocumentGameOverScreen>(true);
                uIDocumentGameOverScreen.ShowGameOverScreen();
                Die();
            }
            else
            {
                animator.SetTrigger(hashTakingDamage);
            }
        }
    }
}
