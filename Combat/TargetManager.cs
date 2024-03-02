using System.Collections;
using System.Linq;
using AF.Characters;
using AF.Companions;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Combat
{

    public class TargetManager : MonoBehaviour
    {

        public UnityEvent onTargetSet_Event;

        [Header("Components")]
        public CharacterBaseManager currentTarget;

        public CharacterManager characterManager;

        [Header("Faction Settings")]
        public CharacterFaction[] characterFactions;

        [Header("Combat Start Settings")]
        bool hasBeenInCombat = false;
        public float delayWhenBeginningCombatForFirstTime = 1f;

        // Scene Reference
        PlayerManager playerManager;
        CompanionsSceneManager companionsSceneManager;

        public void SetTarget(CharacterBaseManager target)
        {
            if (!CanSetTarget())
            {
                return;
            }

            if (currentTarget == target)
            {
                return;
            }

            if (characterFactions.Length > 0 && characterFactions.Contains(target.characterFaction))
            {
                return;
            }

            if (!hasBeenInCombat)
            {
                hasBeenInCombat = true;

                IEnumerator PrepareCombat_Coroutine()
                {
                    yield return new WaitForSeconds(delayWhenBeginningCombatForFirstTime);
                    HandleSetTarget(target);
                }

                StartCoroutine(PrepareCombat_Coroutine());
            }
            else
            {
                HandleSetTarget(target);
            }
        }

        void HandleSetTarget(CharacterBaseManager target)
        {
            currentTarget = target;

            onTargetSet_Event?.Invoke();

            if (characterManager.partners.Length > 0)
            {
                foreach (var combatPartner in characterManager.partners)
                {
                    combatPartner.targetManager.SetTarget(target);
                }
            }

            // Edge case to check if it's player
            if (target is PlayerManager)
            {
                NotifyCompanions();
            }
        }

        void NotifyCompanions()
        {

            foreach (var companionInstance in GetCompanionsSceneManager().companionInstancesInScene)
            {
                companionInstance.Value.GetComponent<CharacterManager>().targetManager.SetTarget(this.characterManager);
            }
        }

        bool CanSetTarget()
        {
            if (characterManager.characterPosture.isStunned)
            {
                return false;
            }

            return true;
        }

        public void ClearTarget()
        {
            currentTarget = null;
        }

        public bool IsTargetBusy()
        {
            if (currentTarget == null)
            {
                return false;
            }

            return currentTarget.IsBusy();
        }

        public bool IsTargetShooting()
        {
            if (currentTarget is PlayerManager playerManager)
            {
                return playerManager.playerShootingManager.isShooting;
            }

            return false;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void SetPlayerAsTarget()
        {
            SetTarget(GetPlayerManager());
        }

        PlayerManager GetPlayerManager()
        {
            if (playerManager == null)
            {
                playerManager = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include);
            }

            return playerManager;
        }

        CompanionsSceneManager GetCompanionsSceneManager()
        {
            if (companionsSceneManager == null)
            {
                companionsSceneManager = FindAnyObjectByType<CompanionsSceneManager>(FindObjectsInactive.Include);
            }

            return companionsSceneManager;
        }

    }
}
