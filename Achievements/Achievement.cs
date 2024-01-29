using System;
using Steamworks;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Data / New Achievement")]

    public class Achievement : ScriptableObject
    {
        [TextArea] public string description;

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
            catch (Exception)
            {
                Debug.Log("Steam is not initialized");
            }
        }
    }
}
