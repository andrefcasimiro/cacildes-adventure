using UnityEngine;

namespace AF
{

    [CreateAssetMenu(menuName = "Items / Accessory / New Accessory")]
    public class Accessory : ArmorBase
    {

        [Header("Stat Bonuses")]
        public int healthBonus = 0;
        public int magicBonus = 0;
        public int staminaBonus = 0;
        public int physicalAttackBonus = 0;
        public int jumpAttackBonus = 0;
        public float twoHandAttackBonusMultiplier = 0f;

        [Header("Stats")]
        public bool increaseAttackPowerTheLowerTheReputation = false;
        public bool increaseAttackPowerWithLowerHealth = false;

        [Header("Posture")]
        public int postureDamagePerParry = 0;
        public float postureDecreaseRateBonus = 0f;
        public float backStabAngleBonus = 0f;

        [Header("Spells")]
        public float spellDamageBonusMultiplier = 0f;

        [Header("Life")]
        public bool chanceToDoubleCoinsFromFallenEnemies = false;
        public bool chanceToRestoreHealthUponDeath = false;

        [Header("Inventory")]
        public bool chanceToNotLoseItemUponConsumption = false;

    }
}
