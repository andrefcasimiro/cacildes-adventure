using System.Linq;
using AF.Characters;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Combat
{

    public class TargetManager : MonoBehaviour
    {

        public UnityEvent onTargetSet_Event;

        [Header("Components")]
        public CharacterBaseManager currentTarget;

        public CharacterBaseManager characterBaseManager;

        [Header("Faction Settings")]
        public CharacterFaction[] characterFactions;

        [Header("Combat Partners")]
        public TargetManager[] combatPartners;

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

            currentTarget = target;

            onTargetSet_Event?.Invoke();

            if (combatPartners.Length > 0)
            {
                foreach (var combatPartner in combatPartners)
                {
                    combatPartner.SetTarget(target);
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
            foreach (CompanionID companionID in FindObjectsByType<CompanionID>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                companionID.characterManager.targetManager.SetTarget(this.characterBaseManager);
            }
        }

        bool CanSetTarget()
        {
            if (characterBaseManager.characterPosture.isStunned)
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
    }
}
