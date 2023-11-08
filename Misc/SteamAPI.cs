using UnityEngine;
using Steamworks;

namespace AF
{
    public class SteamAPI : MonoBehaviour
    {

        public static SteamAPI instance;

        public enum AchievementName
        {
            EN_GARDE,
            ARMED_TO_THE_TEETH,
            WIZARDING_AROUND,
            GLADIATOR,
            FIRST_SHOT,
            FOREST_BUDDY,
            FRIENDS_FOREVER,
            ANOINTED,
            TROUBLED_YOUTH,
            SLEPBONE_BEAST,
            FALLEN,
            AHOY_MATEY,
            FIRST_STEP,
            THIRSTY,
            WEAPONSMITH,
            EGGS_ARE_BACK,
            CHEFS_KISS,
            WALLBREAKER,
            LITTLE_DEVIL,
            OLD_ENOUGH,
            HELL_LORD_SHASAPTHA,
            SYMPATHY_FOR_THE_DEVIL,
            THE_IMPATIENT,
            HIGHWAY_TO_HELL,
            ROBERTO,
            DAMSEL_IN_DISTRESS,
            MAERIMOND,
            YOU_DIED,
            NONE,
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
            }

        }

        public bool IsAchievementCompleted(AchievementName achievementName)
        {
            bool achievementCompleted;

            if (SteamManager.Initialized)
            {
                SteamUserStats.GetAchievement(achievementName.ToString(), out achievementCompleted);
            }
            else
            {
                return false;
            }

            return achievementCompleted;
        }

        public void SetAchievementProgress(AchievementName achievementName, int progress)
        {
            if (SteamManager.Initialized)
            {
                if (IsAchievementCompleted(achievementName))
                {
                    return;
                }

                bool hasAchievement = false;
                SteamUserStats.GetAchievement(achievementName.ToString(), out hasAchievement);

                if (hasAchievement)
                {
                    return;
                }


                SteamUserStats.SetAchievement(achievementName.ToString());
                SteamUserStats.StoreStats();
            }
        }

    }
}
