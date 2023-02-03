using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public enum Faction
    {
        NONE,
        // Slepbone guild
        FOREST_WANDERERS,
        // Includes soldiers in every village and city
        ROYAL_ARMY,
    }



    [System.Serializable]
    public class FactionEntry {
        public Faction faction;
        public int playerReputationWithinFaction = 0;
        public bool isGood = true; // If false, faction will use a negative reputation system
        public int minimumRequiredReputationToInteract = 0;
    }

    public class FactionManager : MonoBehaviour
    {

    }
}
