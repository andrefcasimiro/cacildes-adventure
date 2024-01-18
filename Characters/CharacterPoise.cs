using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class CharacterPoise : MonoBehaviour
    {
        public int currentPoiseHitCount = 0;

        [Header("Components")]
        public CharacterBaseManager characterManager;

        [Header("Settings")]
        [Tooltip("How many hits can the enemy take and continuing attacking")]
        public int maxPoiseHits = 3;
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

            currentPoiseHitCount = Mathf.Clamp(currentPoiseHitCount + 1 + poiseDamage, 0, maxPoiseHits);

            if (ResetPoiseCoroutine != null)
            {
                StopCoroutine(ResetPoiseCoroutine);
            }

            if (currentPoiseHitCount >= maxPoiseHits)
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
    }

}
