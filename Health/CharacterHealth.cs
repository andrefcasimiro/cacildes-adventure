
using AF.Companions;
using AF.Events;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Health
{
    public class CharacterHealth : CharacterBaseHealth
    {
        public CharacterManager characterManager;
        public CompanionsDatabase companionsDatabase;

        [HideInInspector] public UnityEvent onHealthSettingsChanged;

        [SerializeField]
        protected int maxHealth = 100;

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

        [Header("Options")]
        public int bonusHealth = 0;
        public int bonusHealthFromCompanions = 0;


        public void Awake()
        {
            CurrentHealth = GetMaxHealth();

            EventManager.StartListening(EventMessages.ON_PARTY_CHANGED, UpdateHealthSettings);
        }

        void UpdateHealthSettings()
        {
            if (CurrentHealth <= 0)
            {
                return;
            }

            bool wasFullHealthBeforeUpdate = CurrentHealth >= GetMaxHealth();

            this.bonusHealthFromCompanions = (
                !characterManager.IsCompanion() && companionsDatabase.TryGetCompanionCount(out int count) && count > 0) ? HealthUtils.GetExtraHealthBasedOnCompanionsInParty(count) : 0;

            if (wasFullHealthBeforeUpdate)
            {
                CurrentHealth = GetMaxHealth();
            }

            onHealthSettingsChanged?.Invoke();
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

                CheckIfHasBeenKilledWithRightWeapon();

                EventManager.EmitEvent(EventMessages.ON_CHARACTER_KILLED);
                onDeath?.Invoke();
            }
        }

        public override int GetMaxHealth()
        {
            return maxHealth + bonusHealth + bonusHealthFromCompanions;
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

        public override void SetCurrentHealth(float value)
        {
            this.CurrentHealth = value;
        }

        public override void SetMaxHealth(int value)
        {
            this.maxHealth = value;
        }

        public void IncreaseBonusHealth(int value)
        {
            this.bonusHealth += value;
        }
    }

}
