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

        NotificationManager notificationManager;

        private void Awake()
        {
            notificationManager = FindObjectOfType<NotificationManager>(true);
        }

        public override void TakeDamage(float damage, Transform attackerTransform, string attackerName, AudioClip weaponSwingSfx)
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
                return;
            }

            if (player.IsDodging())
            {
                return;
            }

            combatable.SetCurrentHealth(combatable.GetCurrentHealth() - damage);

            notificationManager.ShowNotification("Cacildes received " + damage + " damage from " + attackerName);

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
                Die();
            }
            else
            {
                animator.SetTrigger(hashTakingDamage);
            }
        }
    }
}
