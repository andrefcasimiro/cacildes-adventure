using AF.Stats;
using UnityEngine;

namespace AF
{
    public class PlayerPoise : CharacterAbstractPoise
    {
        public PlayerStatsDatabase playerStatsDatabase;
        public StatsBonusController statsBonusController;

        public PlayerManager playerManager;

        public override void ResetStates()
        {
        }


        public override bool CanCallPoiseDamagedEvent()
        {
            return playerManager.thirdPersonController.isSwimming == false;
        }

        public override int GetMaxPoiseHits()
        {
            return 1 + GetExtraPoiseBasedOnStats() + statsBonusController.equipmentPoise;
        }

        int GetExtraPoiseBasedOnStats()
        {
            return (int)Mathf.Round(playerStatsDatabase.endurance / 5f) + (int)Mathf.Round(playerStatsDatabase.strength / 2.5f); ;
        }
    }
}
