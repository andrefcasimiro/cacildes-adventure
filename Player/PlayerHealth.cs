using AF.Health;
using AF.Stats;
using UnityEngine;

namespace AF
{
    public class PlayerHealth : CharacterBaseHealth
    {

        [Header("Components")]
        public StatsBonusController playerStatsBonusController;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;

        private void Awake()
        {
            // Initialize Health
            if (playerStatsDatabase.currentHealth == -1)
            {
                playerStatsDatabase.currentHealth = GetMaxHealth();
            }
        }


        public override int GetMaxHealth()
        {
            return playerStatsDatabase.maxHealth + (int)((playerStatsDatabase.vitality + playerStatsBonusController.vitalityBonus) * playerStatsDatabase.levelMultiplierForHealth);
        }


        public void SubtractAmountMultipliedByTimeDeltaTime(float amount)
        {
            TakeDamage(amount * Time.deltaTime);
        }

        public void RestoreHealthPercentage(int amount)
        {
            var percentage = GetMaxHealth() * amount / 100;
            var nextValue = Mathf.Clamp(
                playerStatsDatabase.currentHealth + percentage, 0, GetMaxHealth());

            playerStatsDatabase.currentHealth = nextValue;
        }

        public float GetHealthPointsForGivenVitality(int vitality)
        {
            return GetCurrentHealth() + (int)(vitality * playerStatsDatabase.levelMultiplierForHealth);
        }

        public override void RestoreHealth(float value)
        {
            playerStatsDatabase.currentHealth = Mathf.Clamp(
                playerStatsDatabase.currentHealth + value, 0, GetMaxHealth());

            onRestoreHealth?.Invoke();
        }

        public override void TakeDamage(float value)
        {
            if (value <= 0)
            {
                return;
            }

            playerStatsDatabase.currentHealth = Mathf.Clamp(
                playerStatsDatabase.currentHealth - value, 0, GetMaxHealth());

            onTakeDamage?.Invoke();

            if (GetCurrentHealth() <= 0)
            {
                HandleDeath();
            }
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="amount"></param>
        public void TakeDamageWithoutOnTakeDamageEvent(float amount)
        {
            playerStatsDatabase.currentHealth = Mathf.Clamp(
                playerStatsDatabase.currentHealth - amount, 0, GetMaxHealth());

            if (GetCurrentHealth() <= 0)
            {
                HandleDeath();
            }
        }

        void HandleDeath()
        {
            onDeath?.Invoke();
        }

        public override float GetCurrentHealth()
        {
            return playerStatsDatabase.currentHealth;
        }

        public float GetExtraAttackBasedOnCurrentHealth()
        {
            var percentage = playerStatsDatabase.currentHealth * 100 / GetMaxHealth() * 0.01;

            if (percentage > 0.9)
            {
                return 0;
            }
            else if (percentage > 0.8)
            {
                return 0.05f;
            }
            else if (percentage > 0.7)
            {
                return 0.1f;
            }
            else if (percentage > 0.6)
            {
                return 0.2f;
            }
            else if (percentage > 0.5)
            {
                return 0.5f;
            }
            else if (percentage > 0.4)
            {
                return 0.6f;
            }
            else if (percentage > 0.3)
            {
                return 0.8f;
            }
            else if (percentage > 0.2)
            {
                return 1.2f;
            }
            else if (percentage > 0.1)
            {
                return 1.5f;
            }
            else if (percentage > 0)
            {
                return 2f;
            }

            return 0f;
        }

        public override void RestoreFullHealth()
        {
            RestoreHealthPercentage(100);
        }

        public override void SetCurrentHealth(float value)
        {
            this.playerStatsDatabase.currentHealth = value;
        }

        public override void SetMaxHealth(int value)
        {
        }
    }

}
