using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using AF.Stats;
using AF.Health;
using System.Collections;
using UnityEngine.Events;

namespace AF
{
    public class RageManager : MonoBehaviour
    {
        [Header("Components")]
        public PlayerManager playerManager;

        [Header("Rage")]
        public Coroutine RageCoroutine;
        public int maxRage = 10;
        public float rageDelayInSecondsBeforeRemovingOneUnit = 3f;
        public int rageCount = 0;
        public float rageCountMultiplier = 25f;

        public UnityEvent onLowRage;
        public UnityEvent onMidRage;
        public UnityEvent onHighRage;

        bool shouldResetRage = false;

        public void ResetStates()
        {
            CheckIfShouldResetRage();
        }

        public int GetRageBonus()
        {
            int rageBonus = 0;

            if (rageCount > 0)
            {
                rageBonus = (int)(rageCount * rageCountMultiplier);

                EvaluateRageAttack();

                shouldResetRage = true;
            }

            return rageBonus;
        }

        void CheckIfShouldResetRage()
        {
            if (shouldResetRage)
            {
                ResetRage();
                shouldResetRage = false;
            }
        }

        public void IncrementRage()
        {
            if (!CanRage())
            {
                return;
            }

            if (rageCount > maxRage)
            {
                return;
            }

            rageCount++;

            if (RageCoroutine != null)
            {
                StopCoroutine(RageCoroutine);
            }

            RageCoroutine = StartCoroutine(HandleRage_Coroutine());
        }


        IEnumerator HandleRage_Coroutine()
        {
            while (rageCount > 0)
            {
                yield return new WaitForSeconds(rageDelayInSecondsBeforeRemovingOneUnit);
                rageCount--;
            }

            if (rageCount <= 0) { rageCount = 0; }
        }

        bool CanRage()
        {
            if (playerManager.health.GetCurrentHealth() <= 0)
            {
                return false;
            }

            return playerManager.statsBonusController.canRage;
        }

        void ResetRage()
        {
            if (RageCoroutine != null)
            {
                StopCoroutine(RageCoroutine);
            }

            rageCount = 0;
        }

        public void EvaluateRageAttack()
        {
            if (rageCount <= 0)
            {
                return;
            }

            if (rageCount <= 1)
            {
                onLowRage?.Invoke();
            }
            else if (rageCount <= 3)
            {
                onMidRage?.Invoke();
            }
            else
            {
                onHighRage?.Invoke();
            }
        }
    }
}
