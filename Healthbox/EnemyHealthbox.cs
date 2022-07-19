using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AF
{
    public class EnemyHealthbox : Healthbox
    {
        [Header("UI")]
        public Slider healthBarSlider;

        NotificationManager notificationManager;

        private void Awake()
        {
            notificationManager = FindObjectOfType<NotificationManager>(true);
        }

        private void Start()
        {
            healthBarSlider.maxValue = combatable.GetMaxHealth() * 0.01f;
            healthBarSlider.value = combatable.GetCurrentHealth() * 0.01f;
        }

        private void Update()
        {
            healthBarSlider.value = combatable.GetCurrentHealth() * 0.01f;
        }

        public override void TakeDamage(float damage, Transform attackerTransform, string attackerName, AudioClip weaponSwingSfx)
        {
            if (character.IsDodging()) { return; }

            combatable.SetCurrentHealth(combatable.GetCurrentHealth() - damage);

            notificationManager.ShowNotification(
                character.name + " received " + damage + " damage from Cacildes"
            );

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
