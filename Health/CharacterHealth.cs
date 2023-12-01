
using UnityEngine;
using UnityEngine.Events;

namespace AF.Health
{
    public class CharacterHealth : CharacterBaseHealth
    {
        float m_currentHealth;
        protected float CurrentHealth
        {
            get
            {
                return m_currentHealth;
            }

            set
            {
                m_currentHealth = Mathf.Clamp(value, 0, GetMaxHealth());
            }
        }

        [Header("Events")]
        public UnityEvent onRevive;

        public void Awake()
        {
            CurrentHealth = GetMaxHealth();
        }

        public override void RestoreHealth(int value)
        {
            CurrentHealth += value;

            onRestoreHealth?.Invoke();
        }

        public override void TakeDamage(float value)
        {
            if (value <= 0)
            {
                return;
            }

            CurrentHealth -= value;
            onTakeDamage?.Invoke();

            if (CurrentHealth <= 0)
            {
                onDeath?.Invoke();
            }
        }

        public override int GetMaxHealth()
        {
            return maxHealth;
        }

        public override float GetCurrentHealth()
        {
            return CurrentHealth;
        }

        public override void RestoreFullHealth()
        {
            RestoreHealth(GetMaxHealth());
        }

        public void Revive()
        {
            RestoreFullHealth();
            onRevive?.Invoke();
        }
    }

}
