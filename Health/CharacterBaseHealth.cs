
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AF.Health
{
    public abstract class CharacterBaseHealth : MonoBehaviour
    {

        [Header("Events")]
        public UnityEvent onStart;
        public UnityEvent onTakeDamage;
        public UnityEvent onRestoreHealth;
        public UnityEvent onDeath;

        [Header("Sounds")]
        public AudioClip postureHitSfx;
        public AudioClip postureBrokeSfx;
        public AudioClip deathSfx;
        public AudioClip dodgeSfx;
        public AudioSource audioSource;

        private void Start()
        {
            onStart?.Invoke();
        }

        public abstract void RestoreHealth(float value);
        public abstract void RestoreFullHealth();

        public float GetCurrentHealthPercentage()
        {
            return GetCurrentHealth() * 100 / GetMaxHealth();
        }

        public abstract void TakeDamage(float value);

        public abstract float GetCurrentHealth();
        public abstract void SetCurrentHealth(float value);

        public abstract int GetMaxHealth();
        public abstract void SetMaxHealth(int value);

        public void PlayPostureHit()
        {
            if (audioSource != null && postureHitSfx != null && Random.Range(0, 100f) >= 50f)
            {
                audioSource.pitch = Random.Range(0.91f, 1.05f);
                audioSource.PlayOneShot(postureHitSfx);
            }
        }
        public void PlayPostureBroke()
        {
            if (audioSource != null && postureBrokeSfx != null)
            {
                audioSource.PlayOneShot(postureBrokeSfx);
            }
        }
        public void PlayDodge()
        {
            if (audioSource != null && dodgeSfx != null)
            {
                audioSource.PlayOneShot(dodgeSfx);
            }
        }
        public void PlayDeath()
        {
            if (audioSource != null && deathSfx != null)
            {
                audioSource.PlayOneShot(deathSfx);
            }
        }
    }

}
