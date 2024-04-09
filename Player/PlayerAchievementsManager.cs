using UnityEngine;

namespace AF
{
    public class PlayerAchievementsManager : MonoBehaviour
    {
        [Header("Inventory")]
        public Achievement achievementOnAcquiringFirstWeapon;
        public Achievement achievementOnAcquiringTenWeapons;
        public Achievement achievementOnAcquiringFirstSpell;
        [Header("Crafting")]
        public Achievement achievementForBrewingFirstPotion;
        public Achievement achievementForCookingFirstMeal;
        public Achievement achievementForUpgradingFirstWeapon;

    }
}
