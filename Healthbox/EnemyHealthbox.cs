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

        public int maxPostureHits = 1;
        int currentPostureHitCount = 0;
        float timerBeforeReset;
        public float maxTimeBeforeReset = 15f;

        bool hasInvokedOnDeathEvent = false;
        
        private void Awake()
        {
            notificationManager = FindObjectOfType<NotificationManager>(true);
        }

        private void Start()
        {
            if (healthBarSlider != null)
            {
                healthBarSlider.maxValue = combatable.GetMaxHealth() * 0.01f;
                healthBarSlider.value = combatable.GetCurrentHealth() * 0.01f;
            }

            this.TryGetComponent(out enemy);
        }

        private void Update()
        {
            if (maxPostureHits > 1)
            {
                timerBeforeReset += Time.deltaTime;

                if (timerBeforeReset >= maxTimeBeforeReset)
                {
                    currentPostureHitCount = 0;
                    timerBeforeReset = 0f;
                }
            }

            if (healthBarSlider != null)
            {
                healthBarSlider.value = combatable.GetCurrentHealth() * 0.01f;
            }
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

            if (combatable.GetCurrentHealth() <= 0)
            {

                Enemy enemy = (Enemy)this.character;

                Loot loot = enemy.GetComponent<Loot>();
                if (loot != null)
                {
                    StartCoroutine(GiveLoot(loot));
                }

                Die();
            }
            else
            {
                currentPostureHitCount++;

                if (currentPostureHitCount >= maxPostureHits)
                {
                    currentPostureHitCount = 0;
                    animator.SetTrigger(hashTakingDamage);
                    StartCoroutine(PlayHurtSfx());
                }
            }
        }

        IEnumerator GiveLoot(Loot loot)
        {
            yield return new WaitForSeconds(1f);

            PlayerStatsManager.instance.currentExperience += enemy.experienceGained;

            notificationManager.NotifyCoins(enemy.experienceGained);

            loot.GetLoot();
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
