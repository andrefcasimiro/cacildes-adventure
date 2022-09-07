using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AF
{
    public class EnemyHealthbox : Healthbox
    {
        [Header("UI")]
        public Slider healthBarSlider;
        public FloatingText damageFloatingText;

        NotificationManager notificationManager;

        public bool isBlocking = false;
        public bool isDodging = false;
        public bool isParriable = false;

        Enemy enemy;

        private void Awake()
        {
            notificationManager = FindObjectOfType<NotificationManager>(true);
        }

        private void Start()
        {
            healthBarSlider.maxValue = combatable.GetMaxHealth() * 0.01f;
            healthBarSlider.value = combatable.GetCurrentHealth() * 0.01f;

            this.TryGetComponent(out enemy);
        }

        private void Update()
        {
            healthBarSlider.value = combatable.GetCurrentHealth() * 0.01f;
        }

        public override void TakeDamage(float damage, Transform attackerTransform, AudioClip weaponSwingSfx)
        {
            if (!this.enabled)
            {
                return;
            }

            if (isDodging) {
                return;
            }

            if (isBlocking)
            {
                Instantiate(enemy.blockParticleEffect, enemy.shield.transform.position, Quaternion.identity);
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

                Enemy enemy = (Enemy)this.character;

                PlayerStatsManager.instance.currentExperience += enemy.experienceGained;

                notificationManager.ShowNotification("Cacildes received " + enemy.experienceGained + " points of experience");

                Loot loot = enemy.GetComponent<Loot>();
                if (loot != null)
                {
                    loot.GetLoot();
                }

                Die();
            }
            else
            {
                animator.SetTrigger(hashTakingDamage);
            }
        }

        #region Animation Clips
        // DODGING
        public void ActivateDodge()
        {
            isDodging = true;
        }
        public void DeactivateDodge()
        {
            isDodging = false;
        }

        // BLOCK
        public void ActivateBlock()
        {
            isBlocking = true;
        }

        public void DeactivateBlock()
        {
            isBlocking = false;
        }
        
        // RECEIVE PARRY CHANCES
        public void ActivateParriable()
        {
            isParriable = true;
        }

        public void DeactivateParriable()
        {
            isParriable = false;
        }
        #endregion

    }
}
