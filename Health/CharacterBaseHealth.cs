
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AF.Health
{
    public abstract class CharacterBaseHealth : MonoBehaviour
    {
        [SerializeField]
        protected int maxHealth = 100;

        [Header("Events")]
        public UnityEvent onStart;
        public UnityEvent onTakeDamage;
        public UnityEvent onRestoreHealth;
        public UnityEvent onDeath;

        private void Start()
        {
            onStart?.Invoke();
        }

        public abstract void RestoreHealth(int value);

        public float GetCurrentHealthPercentage()
        {
            return GetCurrentHealth() * 100 / GetMaxHealth();
        }

        public abstract void TakeDamage(float value);

        public abstract float GetCurrentHealth();

        public abstract int GetMaxHealth();
    }

}
