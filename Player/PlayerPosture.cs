using AF.Stats;
using UnityEngine;

namespace AF
{
    public class PlayerPosture : CharacterAbstractPosture
    {
        public PlayerStatsDatabase playerStatsDatabase;
        public StatsBonusController statsBonusController;

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
    }
}
