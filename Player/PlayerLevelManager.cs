using UnityEngine;

namespace AF
{
    public class PlayerLevelManager : MonoBehaviour
    {
        public PlayerStatsDatabase playerStatsDatabase;

        public int GetCurrentLevel()
        {
            return playerStatsDatabase.GetCurrentLevel();
        }
        public float GetRequiredExperienceForNextLevel()
        {
            return LevelUtils.GetRequiredExperienceForLevel(this.GetCurrentLevel() + 1);
        }
    }
}
