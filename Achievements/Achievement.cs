using System;
using Steamworks;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Data / New Achievement")]

    public class Achievement : ScriptableObject
    {
        public void AwardAchievement()
        {
            try
            {
                if (!SteamManager.Initialized)
                {
                    return;
                }

                SteamUserStats.GetAchievement(name, out bool achievementCompleted);
                if (achievementCompleted)
                {
                    return;
                }

                SteamUserStats.SetAchievement(name);
                SteamUserStats.StoreStats();
            }
            catch (Exception ex)
            {
                Debug.Log("An error occurred while awarding achievement '" + name + "': " + ex.Message);
            }
        }
    }
}
