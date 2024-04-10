﻿using UnityEngine;

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

        [Header("Stats")]
        public bool increaseAttackPowerTheLowerTheReputation = false;
        public bool increaseAttackPowerWithLowerHealth = false;

        [Header("Posture")]
        public int postureDamagePerParry = 0;

        [Header("Spells")]
        public float spellDamageBonusMultiplier = 0f;

        [Header("Life")]
        public bool chanceToDoubleCoinsFromFallenEnemies = false;

    }
}
