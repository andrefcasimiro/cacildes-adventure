using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class EnemyTargetController : MonoBehaviour
    {
        // When a companion attacks, focus on a companion for a short period of time
        [Header("Companion AI")]
        public bool ignoreCompanions = false;
        [Range(0, 100)] public float focusOnCompanionWeight = 50;
        public CompanionManager currentCompanion;
        public float maxTimeFocusedOnCompanions = 10f;
        float focusedOnCompanionsTimer = Mathf.Infinity;

        EnemyManager enemyManager => GetComponent<EnemyManager>();

        private void Update()
        {
            UpdateCompanionsFocusAI();
        }

        public void UpdateCompanionsFocusAI()
        {
            if (focusedOnCompanionsTimer < maxTimeFocusedOnCompanions)
            {
                focusedOnCompanionsTimer += Time.deltaTime;
            }
            else if (focusedOnCompanionsTimer >= maxTimeFocusedOnCompanions && currentCompanion != null)
            {
                BreakCompanionFocus();
            }
        }

        public void FocusOnCompanion(CompanionManager companionManager)
        {
            var dice = UnityEngine.Random.Range(0, 100);

            if (dice > focusOnCompanionWeight)
            {
                return;
            }

            if (this.ignoreCompanions)
            {
                return;
            }

            focusedOnCompanionsTimer = 0f;

            currentCompanion = companionManager;

            enemyManager.agent.SetDestination(currentCompanion.transform.position);
        }

        public void BreakCompanionFocus()
        {
            currentCompanion = null;

            // Refocus on playerManager
            enemyManager.agent.SetDestination(enemyManager.player.transform.position);
        }

    }

}
