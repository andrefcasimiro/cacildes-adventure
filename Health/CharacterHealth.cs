
using UnityEngine;
using UnityEngine.Events;

namespace AF.Health
{
    public class CharacterHealth : MonoBehaviour
    {
        [SerializeField]
        int m_maxHealth = 100;

        public int MaxHealth
        {
            get
            {
                return m_maxHealth;
            }

            set
            {
                m_maxHealth = value;
            }
        }

        int m_currentHealth;
        public int CurrentHealth
        {
            get
            {
                return m_currentHealth;
            }

            set
            {
                m_currentHealth = Mathf.Clamp(value, 0, MaxHealth);
            }
        }


        [Header("Events")]
        public UnityEvent onStart;
        public UnityEvent onTakeDamage;
        public UnityEvent onRestoreHealth;
        public UnityEvent onDeath;

        void Awake()
        {
            CurrentHealth = MaxHealth;
        }

        private void Start()
        {
            onStart?.Invoke();
        }

        public void RestoreHealth(int value)
        {
            CurrentHealth += value;

            onRestoreHealth?.Invoke();
        }

        public void TakeDamage(int value)
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

    }

}
