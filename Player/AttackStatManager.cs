using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using AF.Stats;

namespace AF
{
    public class AttackStatManager : MonoBehaviour
    {
        // RECOMMENDED ATTACK FORMULA:
        // STR LEVEL * levelMultiplier * weaponScaling
        // A weapon that has S scaling with a levelmultiplier of 3.25 produces:
        // 1 * 3.25 * 2.4 = 8
        // 4 * 3.25 * 2.4 = 31
        // 8 * 3.25 * 2.4 = 62
        // 16 * 3.25 * 2.4 = 125
        // This gives good values, similar to Dark Souls

        [Header("Scaling Multipliers")]
        public float E = 0.25f;
        public float D = 1f;
        public float C = 1.3f;
        public float B = 1.65f;
        public float A = 1.95f;
        public float S = 2.25f;

        private Dictionary<string, float> scalingDictionary = new();

        [Header("Status attack bonus")]
        [Tooltip("Increased by buffs like potions, or equipment like accessories")]
        public float physicalAttackBonus = 0f;

        [Header("Physical Attack")]
        public int basePhysicalAttack = 100;
        public float levelMultiplier = 3.25f;

        public float jumpAttackMultiplier = 2.25f;

        public ThirdPersonController thirdPersonController;
        public PlayerCombatController playerCombatController;
        public DodgeController dodgeController;
        public EquipmentGraphicsHandler equipmentGraphicsHandler;
        public HealthStatManager healthStatManager;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;

        [Header("Components")]
        public StatsBonusController playerStatsBonusController;

        private void Start()
        {
            this.scalingDictionary.Add("E", E);
            this.scalingDictionary.Add("D", D);
            this.scalingDictionary.Add("C", C);
            this.scalingDictionary.Add("B", B);
            this.scalingDictionary.Add("A", A);
            this.scalingDictionary.Add("S", S);
        }

        public bool IsHeavyAttacking()
        {
            return playerCombatController.isHeavyAttacking;
        }

        public bool IsJumpAttacking()
        {
            return false;
            //            return playerCombatController.IsJumpAttacking() || playerCombatController.IsStartingJumpAttack();
        }

        public int GetCurrentPhysicalAttack()
        {
            int heavyAttackBonus = 0;
            if (Player.instance.equippedWeapon == null && playerCombatController.isHeavyAttacking)
            {
                heavyAttackBonus = playerCombatController.unarmedHeavyAttackBonus;
            }

            var value = basePhysicalAttack;

            if (IsJumpAttacking())
            {
                value = Mathf.FloorToInt(value * jumpAttackMultiplier);
            }

            if (dodgeController.IsRollAttacking())
            {
                value += dodgeController.dodgeAttackBonus;
            }

            return (int)Mathf.Round(
                Mathf.Ceil(
                    value
                        + (playerStatsDatabase.strength * levelMultiplier)
                        + (playerStatsDatabase.dexterity * levelMultiplier)
                        + (playerStatsBonusController.strengthBonus * levelMultiplier)
                        + (playerStatsBonusController.dexterityBonus * levelMultiplier)
                    ) + physicalAttackBonus + heavyAttackBonus
                );
        }


        public int GetCurrentPhysicalAttackForGivenStrengthAndDexterity(int strength, int dexterity)
        {
            return (int)Mathf.Round(
                Mathf.Ceil(
                    basePhysicalAttack
                        + (strength * levelMultiplier)
                        + (dexterity * levelMultiplier)
                    )
                );
        }


        #region Weapon Attack
        public int CompareWeapon(Weapon weaponToCompare)
        {
            if (Player.instance.equippedWeapon == null)
            {
                return 1;
            }

            var weaponToCompareAttack = GetWeaponAttack(weaponToCompare);
            var currentWeaponAttack = GetWeaponAttack(Player.instance.equippedWeapon);

            if (weaponToCompareAttack > currentWeaponAttack)
            {
                return 1;
            }

            if (weaponToCompareAttack == currentWeaponAttack)
            {
                return 0;
            }

            return -1;
        }

        public int GetWeaponAttack(Weapon weapon)
        {

            float strengthBonusFromWeapon = GetStrengthBonusFromWeapon(weapon);
            float dexterityBonus = GetDexterityBonusFromWeapon(weapon);
            float intelligenceBonus = GetIntelligenceBonusFromWeapon(weapon);

            var value = (int)(
                GetCurrentPhysicalAttack()
                + weapon.GetWeaponAttack()
                + GetStrengthBonusFromWeapon(weapon)
                + GetDexterityBonusFromWeapon(weapon)
                + GetIntelligenceBonusFromWeapon(weapon)
            );


            if (playerCombatController.isHeavyAttacking)
            {
                int heavyAttackBonus = weapon.heavyAttackBonus;
                value += heavyAttackBonus;
            }

            if (IsJumpAttacking())
            {
                value = Mathf.FloorToInt(value * jumpAttackMultiplier);


                if (Player.instance.equippedAccessories.Count > 0)
                {
                    var attackBonuses = Player.instance.equippedAccessories.Sum(x => x.jumpAttackBonus);
                    value += attackBonuses;
                }

            }

            if (weapon.halveDamage)
            {
                return (int)(value / 2);
            }

            if (Player.instance.equippedAccessories.Count > 0 && Player.instance.equippedAccessories.Find(x => x.increaseAttackPowerTheLowerTheReputation) != null)
            {
                if (playerStatsDatabase.GetCurrentReputation() < 0)
                {
                    int extraAttackPower = (int)(Mathf.Abs(playerStatsDatabase.GetCurrentReputation()) * 3.25f);

                    value += extraAttackPower;
                }
            }

            if (Player.instance.equippedAccessories.Count > 0 && Player.instance.equippedAccessories.Find(x => x.increaseAttackPowerWithLowerHealth) != null)
            {
                int extraAttackPower = (int)(value * healthStatManager.GetExtraAttackBasedOnCurrentHealth());

                value += extraAttackPower;
            }

            if (Player.instance.equippedAccessories.Count > 0)
            {
                var attackBonuses = Player.instance.equippedAccessories.Sum(x => x.physicalAttackBonus);
                value += attackBonuses;
            }

            return value;
        }
        #endregion

        #region Scaling

        public int GetStrengthBonusFromWeapon(Weapon weapon)
        {
            return (int)(Mathf.Ceil(((playerStatsDatabase.strength + playerStatsBonusController.strengthBonus) * this.levelMultiplier * this.scalingDictionary[weapon.strengthScaling.ToString()])));
        }

        public float GetDexterityBonusFromWeapon(Weapon weapon)
        {
            return (int)(Mathf.Ceil(((playerStatsDatabase.dexterity + playerStatsBonusController.dexterityBonus) * this.levelMultiplier * this.scalingDictionary[weapon.dexterityScaling.ToString()])));
        }

        public float GetIntelligenceBonusFromWeapon(Weapon weapon)
        {
            return (int)(Mathf.Ceil(((playerStatsDatabase.intelligence + playerStatsBonusController.intelligenceBonus) * this.levelMultiplier * this.scalingDictionary[weapon.intelligenceScaling.ToString()])));
        }

        #endregion

        public float GetArrowDamageBonus()
        {
            return Mathf.Ceil(playerStatsDatabase.dexterity * this.levelMultiplier + playerStatsBonusController.dexterityBonus * this.levelMultiplier);
        }

    }
}
