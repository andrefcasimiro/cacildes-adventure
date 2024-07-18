using AF.Stats;
using UnityEngine;

namespace AF
{
    public class PlayerPosture : CharacterAbstractPosture
    {
        public PlayerStatsDatabase playerStatsDatabase;
        public StatsBonusController statsBonusController;
        public PlayerManager playerManager;

        public override int GetMaxPostureDamage()
        {
            return 100 + GetExtraPostureBasedOnStats();
        }

        int GetExtraPostureBasedOnStats()
        {
            return playerStatsDatabase.strength / 2 * 10;
        }

        public void ResetPosture()
        {
            currentPostureDamage = 0;
        }

        public override float GetPostureDecreateRate()
        {
            return 1.5f + statsBonusController.postureDecreaseRateBonus;
        }

        public override bool CanPlayPostureDamagedEvent()
        {
            return playerManager.thirdPersonController.isSwimming == false;
        }
    }
}
