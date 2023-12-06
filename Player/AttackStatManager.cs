using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using AF.Stats;
using AF.Health;

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

        [Header("Unarmed Attack Options")]
        public int unarmedLightAttackPostureDamage = 18;
        public int unarmedHeavyAttackPostureDamage = 35;

        [Header("Physical Attack")]
        public int basePhysicalAttack = 100;
        public float levelMultiplier = 3.25f;

        public float jumpAttackMultiplier = 2.25f;

        public ThirdPersonController thirdPersonController;
        public PlayerCombatController playerCombatController;
        public DodgeController dodgeController;
        public EquipmentGraphicsHandler equipmentGraphicsHandler;
        public PlayerHealth healthStatManager;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;

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

        public Damage GetAttackDamage()
        {
            Weapon weapon = equipmentDatabase.GetCurrentWeapon();
            if (weapon != null)
            {
                return new Damage(
                    physical: GetWeaponAttack(weapon),
                    fire: (int)weapon.fireAttack,
                    frost: (int)weapon.frostAttack,
                    magic: (int)weapon.magicAttack,
                    lightning: (int)weapon.lightningAttack,
                    postureDamage: (IsHeavyAttacking() || IsJumpAttacking())
                    ? weapon.heavyAttackPostureDamage
                    : weapon.lightAttackPostureDamage,
                    poiseDamage: weapon.poiseDamageBonus,
                    weaponAttackType: weapon.weaponAttackType
                );
            }

            return new Damage(
                physical: GetCurrentPhysicalAttack(),
                fire: 0,
                frost: 0,
                magic: 0,
                lightning: 0,
                postureDamage: (IsHeavyAttacking() || IsJumpAttacking())
                    ? unarmedLightAttackPostureDamage
                    : unarmedHeavyAttackPostureDamage,
                poiseDamage: 1,
                weaponAttackType: WeaponAttackType.Blunt
            );
        }

        public int GetCurrentPhysicalAttack()
        {
            int heavyAttackBonus = 0;
            if (equipmentDatabase.GetCurrentWeapon() == null && playerCombatController.isHeavyAttacking)
            {
                heavyAttackBonus = playerCombatController.unarmedHeavyAttackBonus;
            }

            var value = basePhysicalAttack;

            if (IsJumpAttacking())
            {
                value = Mathf.FloorToInt(value * jumpAttackMultiplier);
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
            if (equipmentDatabase.GetCurrentWeapon() == null)
            {
                return 1;
            }

            var weaponToCompareAttack = GetWeaponAttack(weaponToCompare);
            var currentWeaponAttack = GetWeaponAttack(equipmentDatabase.GetCurrentWeapon());

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

                var jumpAttackBonuses = equipmentDatabase.accessories.Sum(x => x != null ? x.jumpAttackBonus : 0);
                value += jumpAttackBonuses;
            }

            if (weapon.halveDamage)
            {
                return (int)(value / 2);
            }

            if (equipmentDatabase.accessories.FirstOrDefault(x => x != null && x.increaseAttackPowerTheLowerTheReputation) != null)
            {
                if (playerStatsDatabase.GetCurrentReputation() < 0)
                {
                    int extraAttackPower = (int)(Mathf.Abs(playerStatsDatabase.GetCurrentReputation()) * 3.25f);

                    value += extraAttackPower;
                }
            }

            if (equipmentDatabase.accessories.FirstOrDefault(x => x != null && x.increaseAttackPowerWithLowerHealth) != null)
            {
                int extraAttackPower = (int)(value * healthStatManager.GetExtraAttackBasedOnCurrentHealth());

                value += extraAttackPower;
            }


            var attackBonuses = equipmentDatabase.accessories.Sum(x => x != null ? x.physicalAttackBonus : 0);
            value += attackBonuses;

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
