using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public abstract class CharacterAbstractPoise : MonoBehaviour
    {
        public int currentPoiseHitCount = 0;

        [Header("Components")]
        public CharacterBaseManager characterManager;

        [Header("Settings")]
        [Tooltip("How many hits can the enemy take and continuing attacking")]
        public float maxTimeBeforeResettingPoise = 5f;

        [Header("Unity Events")]
        public UnityEvent onPoiseDamagedEvent;

        Coroutine ResetPoiseCoroutine;

        public void TakePoiseDamage(int poiseDamage)
        {
            if (characterManager.characterPosture.isStunned)
            {
                return;
            }

            if (characterManager.health.GetCurrentHealth() <= 0)
            {
                return;
            }

            currentPoiseHitCount = Mathf.Clamp(currentPoiseHitCount + 1 + poiseDamage, 0, GetMaxPoiseHits());

            if (ResetPoiseCoroutine != null)
            {
                StopCoroutine(ResetPoiseCoroutine);
            }

            if (currentPoiseHitCount >= GetMaxPoiseHits())
            {
                currentPoiseHitCount = 0;
                onPoiseDamagedEvent?.Invoke();

                characterManager.health.PlayPostureHit();
            }
            else
            {
                StartCoroutine(ResetPoise());
            }
        }

        IEnumerator ResetPoise()
        {
            yield return new WaitForSeconds(maxTimeBeforeResettingPoise);
            currentPoiseHitCount = 0;
        }

        public abstract int GetMaxPoiseHits();
    }

}
