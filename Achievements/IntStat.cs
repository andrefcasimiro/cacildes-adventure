using System;
using Steamworks;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Data / New Int Stat")]

    public class IntStat : ScriptableObject
    {
        public void UpdateStat()
        {
            try
            {
                if (!SteamManager.Initialized)
                {
                    return;
                }

                int currentValue;
                Steamworks.SteamUserStats.GetStat(name, out currentValue);
                currentValue++;

                Steamworks.SteamUserStats.SetStat(name, currentValue);
                Steamworks.SteamUserStats.StoreStats();
            }
            catch (Exception ex)
            {
                Debug.Log("An error occurred while updating stat '" + name + "': " + ex.Message);
            }
        }
    }
}
