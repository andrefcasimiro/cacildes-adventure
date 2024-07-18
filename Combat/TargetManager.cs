using System.Collections;
using System.Linq;
using AF.Characters;
using AF.Companions;
using AF.Events;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Combat
{

    public class TargetManager : MonoBehaviour
    {
        [Header("Events")]
        public UnityEvent onTargetSet_Event;
        public UnityEvent onAgressiveTowardsPlayer_Event;
        public UnityEvent onClearTarget_Event;


        [Header("Components")]
        public CharacterBaseManager currentTarget;

        public CharacterManager characterManager;

        [Header("Faction Settings")]
        public UnityAction<bool> onAgressiveTowardsPlayer;

        [Header("Combat Start Settings")]
        bool hasBeenInCombat = false;
        public float delayWhenBeginningCombatForFirstTime = 1f;

        // Scene Reference
        PlayerManager playerManager;
        CompanionsSceneManager companionsSceneManager;

        private void Awake()
        {
            EventManager.StartListening(EventMessages.ON_LEAVING_BONFIRE, () =>
            {
                hasBeenInCombat = false;
            });
        }

        public void SetTarget(CharacterBaseManager target)
        {
            SetTarget(target, () => { }, false);
        }

        public void SetTarget(CharacterBaseManager target, bool ignorePostureBroken)
        {
            SetTarget(target, () => { }, ignorePostureBroken);
        }

        public void SetTarget(CharacterBaseManager target, UnityAction onTargetSetCallback, bool ignorePostureBroken)
        {
            if (!CanSetTarget(ignorePostureBroken))
            {
                return;
            }

            if (currentTarget == target)
            {
                return;
            }

            if (characterManager.IsFromSameFaction(target))
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

                    onTargetSetCallback?.Invoke();
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

            if (characterManager != null && characterManager.partners != null && characterManager.partners.Length > 0)
            {
                foreach (var combatPartner in characterManager.partners)
                {
                    if (combatPartner != null && combatPartner.targetManager != null)
                    {
                        combatPartner.targetManager.SetTarget(target);
                    }
                }
            }

            // Edge case to check if it's player
            if (target is PlayerManager)
            {
                NotifyCompanions();

                if (onAgressiveTowardsPlayer != null)
                {
                    onAgressiveTowardsPlayer(true);
                }
                onAgressiveTowardsPlayer_Event?.Invoke();
            }
        }

        void NotifyCompanions()
        {
            foreach (var companionInstance in GetCompanionsSceneManager().companionInstancesInScene)
            {
                companionInstance.Value.GetComponent<CharacterManager>().targetManager.SetTarget(this.characterManager);
            }

            Minion[] minionsInScene = FindObjectsByType<Minion>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var minion in minionsInScene)
            {
                if (minion.TryGetComponent<CharacterManager>(out var charManager))
                {
                    charManager.targetManager.SetTarget(this.characterManager);
                }
            }


        }

        bool CanSetTarget(bool ignorePostureBroken)
        {
            if (ignorePostureBroken == false && characterManager.characterPosture.isStunned)
            {
                return false;
            }

            return true;
        }

        public void ClearTarget()
        {
            currentTarget = null;
            if (onAgressiveTowardsPlayer != null)
            {
                onAgressiveTowardsPlayer(false);
            }
            onClearTarget_Event?.Invoke();
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

        public bool IsTargetOutOfMeleeRange()
        {
            if (currentTarget == null)
            {
                return false;
            }

            float maxDistance = characterManager.agent.stoppingDistance;
            if (characterManager.characterCombatController.currentCombatAction != null)
            {
                maxDistance += characterManager.characterCombatController.currentCombatAction.maximumDistanceToTarget;
            }

            return Vector3.Distance(currentTarget.transform.position, characterManager.transform.position) > maxDistance;
        }

    }
}
