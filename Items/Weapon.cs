using System.Collections.Generic;
using System.Linq;
using AF.Animations;
using AF.Health;
using AYellowpaper.SerializedCollections;
using UnityEditor;
using UnityEngine;

namespace AF
{
    public enum Scaling
    {
        S,
        A,
        B,
        C,
        D,
        E
    }

    public enum WeaponAttackType
    {
        Slash,
        Pierce,
        Blunt,
        Range,
        Staff,
    }

    public enum WeaponElementType
    {
        None,
        Fire,
        Frost,
        Lightning,
        Magic,
        Darkness,
    }

    public enum PushForce
    {
        None = 1,
        Light = 2,
        Medium = 3,
        Large = 4,
        VeryLarge = 5,
        Colossal = 6,
    }

    [System.Serializable]
    public class WeaponUpgradeLevel
    {
        public int goldCostForUpgrade;
        public Damage newDamage;

        public SerializedDictionary<UpgradeMaterial, int> upgradeMaterials;
    }

    [CreateAssetMenu(menuName = "Items / Weapon / New Weapon")]
    public class Weapon : Item
    {
        [Header("Attack")]
        public Damage damage;
        public int heavyAttackBonus;
        public int heavyAttackPostureDamage;

        [Header("Level & Upgrades")]
        public bool canBeUpgraded = true;
        public int level = 1;
        public WeaponUpgradeLevel[] weaponUpgrades;

        //        [Tooltip("How much block hit this weapon does on an enemy shield. Heavier weapons should do at least 2 or 3 hits.")]
        //        public int blockHitAmount = 1;

        //        [Header("Block Absorption")]
        //        [Range(0, 100)] public int blockAbsorption = 75;
        //        public float blockStaminaCost = 30f;


        [Header("Stamina")]
        public int lightAttackStaminaCost = 20;
        public int heavyAttackStaminaCost = 35;

        [Header("Scaling")]
        public Scaling strengthScaling = Scaling.E;
        public Scaling dexterityScaling = Scaling.E;
        public Scaling intelligenceScaling = Scaling.E;


        [Header("Animation Overrides")]
        public List<AnimationOverride> animationOverrides;
        [Tooltip("Optional")] public List<AnimationOverride> twoHandOverrides;
        //        [Tooltip("Optional")] public List<AnimationOverride> blockOverrides;

        [Header("Dual Wielding Options")]
        public bool halveDamage = false;


        [Header("Speed Penalty")]
        [Tooltip("Will be added as a negative speed to the animator when equipped")]
        public float speedPenalty = 0f;
        [Range(0.1f, 2f)] public float oneHandAttackSpeedPenalty = 1f;

        [Header("Weapon Bonus")]
        public int amountOfGoldReceivedPerHit = 0;
        public bool doubleDamageDuringNightTime = false;


        [Header("Jump Attack")]
        public float jumpAttackVelocity = -5f;

        [Header("Is Holy?")]
        public bool isHolyWeapon = false;

#if UNITY_EDITOR
        private void OnEnable()
        {
            // No need to populate the list; it's serialized directly
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                // Clear the list when exiting play mode
                level = 1;
            }
        }
#endif

        public Damage CalculateValue(int currentLevel)
        {

            WeaponUpgradeLevel weaponUpgradeLevel = weaponUpgrades.ElementAtOrDefault(currentLevel - 2);

            if (weaponUpgradeLevel != null)
            {
                return weaponUpgradeLevel.newDamage;
            }

            return this.damage;
        }

        public int GetWeaponAttack()
        {
            return CalculateValue(this.level).physical;
        }
        public int GetWeaponAttackForLevel(int level)
        {
            return CalculateValue(level).physical;
        }
        public int GetWeaponFireAttack()
        {
            return CalculateValue(this.level).fire;
        }
        public int GetWeaponFireAttackForLevel(int level)
        {
            return CalculateValue(level).fire;
        }
        public int GetWeaponFrostAttack()
        {
            return CalculateValue(this.level).frost;
        }
        public int GetWeaponFrostAttackForLevel(int level)
        {
            return CalculateValue(level).frost;
        }
        public int GetWeaponLightningAttack()
        {
            return CalculateValue(this.level).lightning;
        }
        public int GetWeaponLightningAttackForLevel(int level)
        {
            return CalculateValue(level).lightning;
        }

        public int GetWeaponDarknessAttack()
        {
            return CalculateValue(this.level).darkness;
        }

        public int GetWeaponDarknessAttackForLevel(int level)
        {
            return CalculateValue(level).darkness;
        }

        public int GetWeaponMagicAttack()
        {
            return CalculateValue(this.level).magic;
        }

        public int GetWeaponMagicAttackForLevel(int level)
        {
            return CalculateValue(level).magic;
        }


        public string GetFormattedStatusDamages()
        {
            string result = "";

            foreach (var statusEffect in damage.statusEffects)
            {
                if (statusEffect != null)
                {
                    result += $"+{statusEffect.amountPerHit} {statusEffect.statusEffect.name} per HIT\n";
                }
            }

            return result.TrimEnd();
        }

        public bool CanBeUpgradedFurther()
        {
            return canBeUpgraded && weaponUpgrades != null && weaponUpgrades.Length > 0 && this.level > 0 && this.level <= weaponUpgrades.Length;
        }

        public string GetMaterialCostForNextLevel()
        {
            if (CanBeUpgradedFurther() && weaponUpgrades[this.level - 1] != null && weaponUpgrades[this.level - 1].upgradeMaterials != null)
            {
                WeaponUpgradeLevel nextWeaponUpgradeLevel = weaponUpgrades[this.level - 1];
                string text = $"Next Weapon Level: {this.level + 1}\n";

                if (nextWeaponUpgradeLevel.newDamage.physical > 0)
                {
                    text += $"+{nextWeaponUpgradeLevel.newDamage.physical} Physical ATK\n";
                }
                if (nextWeaponUpgradeLevel.newDamage.fire > 0)
                {
                    text += $"+{nextWeaponUpgradeLevel.newDamage.fire} Fire ATK\n";
                }
                if (nextWeaponUpgradeLevel.newDamage.frost > 0)
                {
                    text += $"+{nextWeaponUpgradeLevel.newDamage.frost} Frost ATK\n";
                }
                if (nextWeaponUpgradeLevel.newDamage.lightning > 0)
                {
                    text += $"+{nextWeaponUpgradeLevel.newDamage.lightning} Lightning ATK\n";
                }
                if (nextWeaponUpgradeLevel.newDamage.magic > 0)
                {
                    text += $"+{nextWeaponUpgradeLevel.newDamage.magic} Magic ATK\n";
                }
                if (nextWeaponUpgradeLevel.newDamage.darkness > 0)
                {
                    text += $"+{nextWeaponUpgradeLevel.newDamage.darkness} Darkness ATK\n";
                }

                text += $"Required Gold: {nextWeaponUpgradeLevel.goldCostForUpgrade} Coins\n";
                text += $"Required Items:\n";

                foreach (var upgradeMat in weaponUpgrades[this.level - 1].upgradeMaterials)
                {
                    if (upgradeMat.Key != null)
                    {
                        text += $"- {upgradeMat.Key.name}: x{upgradeMat.Value}\n";
                    }
                }

                return text;
            }
            return "";
        }
    }

}
