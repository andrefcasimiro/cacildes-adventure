using System;
using Steamworks;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Data / New Float Stat")]

    public class FloatStat : ScriptableObject
    {
        public void UpdateStat()
        {
            try
            {
                if (!SteamManager.Initialized)
                {
                    return;
                }

                float currentValue;
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
