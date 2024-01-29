
using AF.Events;
using TigerForge;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace AF.Health
{
    public class CharacterHealth : CharacterBaseHealth
    {
        [SerializeField] float m_currentHealth;
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
        public UnityEvent onHalfHealth;
        bool hasRunHalthHealthEvent = false;
        public UnityEvent onRevive;

        public void Awake()
        {
            CurrentHealth = GetMaxHealth();
        }

        public override void RestoreHealth(float value)
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

            if (hasRunHalthHealthEvent == false && CurrentHealth <= GetMaxHealth() / 2)
            {
                hasRunHalthHealthEvent = true;
                onHalfHealth?.Invoke();
            }

            onTakeDamage?.Invoke();

            if (CurrentHealth <= 0)
            {
                PlayDeath();
                EventManager.EmitEvent(EventMessages.ON_CHARACTER_KILLED);
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
            hasRunHalthHealthEvent = false;
            RestoreFullHealth();
            onRevive?.Invoke();
        }

    }

}
