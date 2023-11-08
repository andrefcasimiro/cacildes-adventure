using UnityEngine;

namespace AF
{
    public class PlayerAchievementsManager : MonoBehaviour
    {
        [Header("Inventory")]
        public Achievement achievementOnAcquiringFirstWeapon;
        public Achievement achievementOnAcquiringTenWeapons;
        public Achievement achievementOnAcquiringFirstSpell;
        public Achievement achievementForStealing;
        public Achievement achievementForDrinkingFirstAlcoholicBeverage;
        [Header("Crafting")]
        public Achievement achievementForBrewingFirstPotion;
        public Achievement achievementForCookingFirstMeal;
        public Achievement achievementForUpgradingFirstWeapon;

        [Header("Combat")]
        public Achievement achievementForBreakingEnemyStance;
        public Achievement achievementForDyingFirstTime;
    }
}
