using System.Collections.Generic;
using System.Linq;
using AF.Animations;
using AF.Health;
using AF.Stats;
using AYellowpaper.SerializedCollections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Settings;

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
        public int heavyAttackPostureDamageBonus;

        [Header("Level & Upgrades")]
        public bool canBeUpgraded = true;
        public int level = 1;
        public WeaponUpgradeLevel[] weaponUpgrades;

        //        [Tooltip("How much block hit this weapon does on an enemy shield. Heavier weapons should do at least 2 or 3 hits.")]
        //        public int blockHitAmount = 1;

        //        [Header("Block Absorption")]
        //        [Range(0, 100)] public int blockAbsorption = 75;
        //        public float blockStaminaCost = 30f;

        [Header("Requirements")]
        public int strengthRequired = 0;
        public int dexterityRequired = 0;
        public int intelligenceRequired = 0;
        public int positiveReputationRequired = 0;
        public int negativeReputationRequired = 0;


        [Header("Stamina")]
        public int lightAttackStaminaCost = 20;
        public int heavyAttackStaminaCost = 35;

        [Header("Scaling")]
        public Scaling strengthScaling = Scaling.E;
        public Scaling dexterityScaling = Scaling.E;
        public Scaling intelligenceScaling = Scaling.E;
        [Header("Weapon Special Options")]
        public int manaCostToUseWeaponSpecialAttack = 0;

        [Header("Animation Overrides")]
        public List<AnimationOverride> animationOverrides;
        [Tooltip("Optional")] public List<AnimationOverride> twoHandOverrides;
        [Tooltip("Optional")] public List<AnimationOverride> blockOverrides;

        [Header("Dual Wielding Options")]
        public bool halveDamage = false;

        [Header("Speed Penalty")]
        [Tooltip("Will be added as a negative speed to the animator when equipped")]
        public float speedPenalty = 0f;
        [Range(0.1f, 2f)] public float oneHandAttackSpeedPenalty = 1f;
        [Range(0.1f, 2f)] public float twoHandAttackSpeedPenalty = 1f;

        [Header("Weapon Bonus")]
        public int amountOfGoldReceivedPerHit = 0;
        public bool doubleCoinsUponKillingEnemies = false;
        public bool doubleDamageDuringNightTime = false;
        public int healthRestoredWithEachHit = 0;

        [Header("Jump Attack")]
        public float jumpAttackVelocity = -5f;

        [Header("Is Holy?")]
        public bool isHolyWeapon = false;

        [Header("Range Category")]
        public bool isCrossbow = false;

        [Header("Block Options")]
        [Range(0, 1f)] public float blockAbsorption = .8f;

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
                string text = $"{LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Next Weapon Level: ")}{this.level + 1}\n";

                /*if (nextWeaponUpgradeLevel.newDamage.physical > 0)
                {
                    text += $"+{nextWeaponUpgradeLevel.newDamage.physical} {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Physical ATK")}\n";
                }
                if (nextWeaponUpgradeLevel.newDamage.fire > 0)
                {
                    text += $"+{nextWeaponUpgradeLevel.newDamage.fire} {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Fire ATK")}\n";
                }
                if (nextWeaponUpgradeLevel.newDamage.frost > 0)
                {
                    text += $"+{nextWeaponUpgradeLevel.newDamage.frost}  {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Frost ATK")}\n";
                }
                if (nextWeaponUpgradeLevel.newDamage.lightning > 0)
                {
                    text += $"+{nextWeaponUpgradeLevel.newDamage.lightning}  {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Lightning ATK")}\n";
                }
                if (nextWeaponUpgradeLevel.newDamage.magic > 0)
                {
                    text += $"+{nextWeaponUpgradeLevel.newDamage.magic}  {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Magic ATK")}\n";
                }
                if (nextWeaponUpgradeLevel.newDamage.darkness > 0)
                {
                    text += $"+{nextWeaponUpgradeLevel.newDamage.darkness}  {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Darkness ATK")}\n";
                }

                if (nextWeaponUpgradeLevel.newDamage.statusEffects != null)
                {
                    foreach (var statusEffect in nextWeaponUpgradeLevel.newDamage.statusEffects)
                    {
                        text += $"+{statusEffect.amountPerHit} {statusEffect.statusEffect.GetName()}  {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Inflicted per Hit")}\n";
                    }
                }*/

                text += $"{LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Required Gold:")} {nextWeaponUpgradeLevel.goldCostForUpgrade} {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Coins")}\n";
                text += $"{LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Required Items:")} \n";

                foreach (var upgradeMat in weaponUpgrades[this.level - 1].upgradeMaterials)
                {
                    if (upgradeMat.Key != null)
                    {
                        text += $"- {upgradeMat.Key.GetName()}: x{upgradeMat.Value}\n";
                    }
                }

                return text;
            }

            return "";
        }

        public bool HasRequirements()
        {
            return strengthRequired != 0 || dexterityRequired != 0 || intelligenceRequired != 0 || positiveReputationRequired != 0 || negativeReputationRequired != 0;
        }

        public bool AreRequirementsMet(StatsBonusController statsBonusController)
        {
            if (strengthRequired != 0 && statsBonusController.GetCurrentStrength() < strengthRequired)
            {
                return false;
            }
            else if (dexterityRequired != 0 && statsBonusController.GetCurrentDexterity() < dexterityRequired)
            {
                return false;
            }
            else if (intelligenceRequired != 0 && statsBonusController.GetCurrentIntelligence() < intelligenceRequired)
            {
                return false;
            }
            else if (positiveReputationRequired != 0 && statsBonusController.GetCurrentReputation() < positiveReputationRequired)
            {
                return false;
            }
            else if (negativeReputationRequired != 0 && statsBonusController.GetCurrentReputation() > -negativeReputationRequired)
            {
                return false;
            }

            return true;
        }

        public string DrawRequirements(StatsBonusController statsBonusController)
        {
            string text = AreRequirementsMet(statsBonusController)
                ? LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Requirements met: ")
                : LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Requirements not met: ");

            if (strengthRequired != 0)
            {
                text += $"  {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Strength Required:")} {strengthRequired}   {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Current:")} {statsBonusController.GetCurrentStrength()}\n";
            }
            if (dexterityRequired != 0)
            {
                text += $"  {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Dexterity Required:")} {dexterityRequired}   {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Current:")} {statsBonusController.GetCurrentDexterity()}\n";
            }
            if (intelligenceRequired != 0)
            {
                text += $"  {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Intelligence Required:")} {intelligenceRequired}   {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Current:")} {statsBonusController.GetCurrentIntelligence()}\n";
            }
            if (positiveReputationRequired != 0)
            {
                text += $"  {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Reputation Required:")} {intelligenceRequired}   {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Current:")} {statsBonusController.GetCurrentReputation()}\n";
            }

            if (negativeReputationRequired != 0)
            {
                text += $"  {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Reputation Required:")} -{negativeReputationRequired}   {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Current:")} {statsBonusController.GetCurrentReputation()}\n";
            }

            return text.TrimEnd();
        }
    }

}
