using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF.UI.EquipmentMenu
{
    public class ItemTooltip : MonoBehaviour
    {
        public const string TOOLTIP = "ItemTooltip";
        VisualElement tooltip;
        VisualElement itemInfo;
        VisualElement tooltipItemSprite;
        Label tooltipItemDescription;
        VisualElement[] tooltipItemInfoChildren;
        // Common
        VisualElement tooltipSpeedPenalty;
        VisualElement tooltipIsHolyWeapon;
        VisualElement tooltipPoise;
        VisualElement tooltipFire, tooltipFrost, tooltipLightning, tooltipMagic, tooltipPoison, tooltipBleed;
        VisualElement tooltipBlockAbsorption;
        // Weapons
        VisualElement tooltipPhysicalAttack;
        VisualElement tooltipWeaponType;
        VisualElement tooltipWeaponSpecial;
        VisualElement tooltipWeaponStrengthScaling, tooltipWeaponDexterityScaling, tooltipWeaponIntelligenceScaling;
        // Armor
        VisualElement tooltipPhysicalDefense;
        VisualElement tooltipAccessoryProperty;
        VisualElement tooltipReputationBonus, tooltipVitality, tooltipEndurance, tooltipStrength, tooltipDexterity, tooltipIntelligence, tooltipGold;
        // Consumables
        VisualElement tooltipConsumableEffect;

        [Header("Components")]
        public AttackStatManager attackStatManager;

        // FIRE COLOR: FF662A
        // FROST COLOR: 60CAFF
        // LIGHTNING COLOR: FFEF5F
        // MAGIC COLOR: F160FF
        // BLEED COLOR: FF0910
        // POISON COLOR: AB44FF

        [Header("UI Documents")]
        public UIDocument uIDocument;
        public VisualElement root;

        [HideInInspector] public bool shouldRerender = true;

        private void OnEnable()
        {
            if (shouldRerender)
            {
                shouldRerender = false;

                SetupRefs();
            }

            tooltip.style.display = DisplayStyle.Flex;
        }

        private void OnDisable()
        {
            tooltip.style.display = DisplayStyle.None;
        }

        public void SetupRefs()
        {
            root = uIDocument.rootVisualElement;

            tooltip = root.Q<VisualElement>(TOOLTIP);
            itemInfo = tooltip.Q<VisualElement>("ItemInfo");
            tooltipItemSprite = itemInfo.Q<VisualElement>("ItemSprite");
            tooltipItemDescription = itemInfo.Q<Label>();
            tooltipItemInfoChildren = tooltip.Q<VisualElement>("ItemAttributes").Children().ToArray();
            tooltipPhysicalAttack = tooltip.Q("PhysicalAttack");
            tooltipPhysicalDefense = tooltip.Q("PhysicalDefense");
            tooltipPhysicalDefense = tooltip.Q("PhysicalDefense");
            tooltipSpeedPenalty = tooltip.Q("SpeedPenalty");
            tooltipIsHolyWeapon = tooltip.Q("IsHolyWeapon");
            tooltipAccessoryProperty = tooltip.Q("AccessoryProperty");
            tooltipConsumableEffect = tooltip.Q("ConsumableEffect");
            tooltipWeaponType = tooltip.Q("WeaponType");
            tooltipWeaponSpecial = tooltip.Q("WeaponSpecial");
            tooltipWeaponStrengthScaling = tooltip.Q("StrengthScaling");
            tooltipWeaponDexterityScaling = tooltip.Q("DexterityScaling");
            tooltipWeaponIntelligenceScaling = tooltip.Q("IntelligenceScaling");
            tooltipBlockAbsorption = tooltip.Q("BlockAbsorption");
            tooltipBleed = tooltip.Q("Bleed");
            tooltipPoison = tooltip.Q("Poison");
            tooltipPoise = tooltip.Q("Poise");
            tooltipFire = tooltip.Q("Fire");
            tooltipFrost = tooltip.Q("Frost");
            tooltipLightning = tooltip.Q("Lightning");
            tooltipMagic = tooltip.Q("Magic");

            tooltipVitality = tooltip.Q("Vitality");
            tooltipEndurance = tooltip.Q("Endurance");
            tooltipStrength = tooltip.Q("Strength");
            tooltipDexterity = tooltip.Q("Dexterity");
            tooltipIntelligence = tooltip.Q("Intelligence");
            tooltipReputationBonus = tooltip.Q("Reputation");
            tooltipGold = tooltip.Q("Gold");
        }

        public void PrepareTooltipForItem(Item item)
        {
            enabled = true;

            foreach (var child in tooltipItemInfoChildren)
            {
                child.style.display = DisplayStyle.None;
            }

            tooltipItemSprite.style.backgroundImage = new StyleBackground(item.sprite);
            tooltipItemDescription.text = item.name.GetEnglishText().ToUpper() + " \n" + '"' + item.itemDescription + '"';

            if (item is Weapon weapon)
            {

                var attack = attackStatManager.GetWeaponAttack(weapon);
                var strengthAttackBonus = attackStatManager.GetStrengthBonusFromWeapon(weapon);
                var dexterityAttackBonus = attackStatManager.GetDexterityBonusFromWeapon(weapon);
                var intelligenceAttackBonus = attackStatManager.GetIntelligenceBonusFromWeapon(weapon);
                attack = (int)(attack - strengthAttackBonus - dexterityAttackBonus - intelligenceAttackBonus);

                if (weapon.halveDamage)
                {
                    attack = (int)(attack / 2);
                }

                tooltipPhysicalAttack.Q<Label>().text = $"+ {attack} (Physical Attack)";

                if (weapon.halveDamage)
                {
                    tooltipPhysicalAttack.Q<Label>().text += " (Halved damage)";
                }

                tooltipPhysicalAttack.style.display = DisplayStyle.Flex;

                if (weapon.speedPenalty > 0)
                {
                    tooltipSpeedPenalty.Q<Label>().text = $"+ {Math.Round(weapon.speedPenalty * 100, 2)}% Equip Load";
                    tooltipSpeedPenalty.style.display = DisplayStyle.Flex;
                    tooltipSpeedPenalty.style.opacity = 1;
                }

                if (weapon.isHolyWeapon)
                {
                    tooltipIsHolyWeapon.Q<Label>().text = $"Holy Weapon";
                    tooltipIsHolyWeapon.style.display = DisplayStyle.Flex;
                    tooltipIsHolyWeapon.style.opacity = 1;
                }

                if (weapon.fireAttack > 0)
                {
                    tooltipFire.Q<Label>().text = $"+ {weapon.fireAttack} Fire Attack";
                    tooltipFire.style.display = DisplayStyle.Flex;
                }

                if (weapon.frostAttack > 0)
                {
                    tooltipFrost.Q<Label>().text = $"+ {weapon.frostAttack} Frost Attack";
                    tooltipFrost.style.display = DisplayStyle.Flex;
                }

                if (weapon.lightningAttack > 0)
                {
                    tooltipLightning.Q<Label>().text = $"+ {weapon.lightningAttack} Lightning Attack";
                    tooltipLightning.style.display = DisplayStyle.Flex;
                }

                if (weapon.magicAttack > 0)
                {
                    tooltipMagic.Q<Label>().text = $"+ {weapon.magicAttack} Magic Attack";
                    tooltipMagic.style.display = DisplayStyle.Flex;
                }

                if (weapon.weaponAttackType == WeaponAttackType.Blunt)
                {
                    tooltipWeaponType.Q<Label>().text = $"Damage Type: Blunt";
                    tooltipWeaponType.style.display = DisplayStyle.Flex;
                }
                if (weapon.weaponAttackType == WeaponAttackType.Pierce)
                {
                    tooltipWeaponType.Q<Label>().text = $"Damage Type: Pierce";
                    tooltipWeaponType.style.display = DisplayStyle.Flex;
                }
                if (weapon.weaponAttackType == WeaponAttackType.Slash)
                {
                    tooltipWeaponType.Q<Label>().text = $"Damage Type: Slash";
                    tooltipWeaponType.style.display = DisplayStyle.Flex;
                }

                if (weapon.weaponSpecial != null)
                {
                    tooltipWeaponSpecial.Q<Label>().text = weapon.weaponSpecialDescription.GetEnglishText();
                    tooltipWeaponSpecial.style.display = DisplayStyle.Flex;
                }

                string strengthScaling = $"+{strengthAttackBonus} Attack [{weapon.strengthScaling}] (Strength Scaling)";
                tooltipWeaponStrengthScaling.Q<Label>().text = strengthScaling;
                tooltipWeaponStrengthScaling.style.display = DisplayStyle.Flex;

                string dexterityScaling = $"+{dexterityAttackBonus} Attack [{weapon.dexterityScaling}] (Dexterity Scaling)";
                tooltipWeaponDexterityScaling.Q<Label>().text = dexterityScaling;
                tooltipWeaponDexterityScaling.style.display = DisplayStyle.Flex;

                string intelligenceScaling = $"+{intelligenceAttackBonus} Attack [{weapon.intelligenceScaling}] (Intelligence Scaling)";
                tooltipWeaponIntelligenceScaling.Q<Label>().text = intelligenceScaling;
                tooltipWeaponIntelligenceScaling.style.display = DisplayStyle.Flex;

                if (weapon.statusEffects != null && weapon.statusEffects.Length > 0)
                {
                    foreach (var statusEffect in weapon.statusEffects)
                    {
                        if (statusEffect.statusEffect.name == "Bleed")
                        {
                            tooltipBleed.Q<Label>().text = $"Bleed per hit: {statusEffect.amountPerHit}";
                            tooltipBleed.style.display = DisplayStyle.Flex;
                        }

                        if (statusEffect.statusEffect.name == "Poison")
                        {
                            tooltipPoison.Q<Label>().text = $"Poison per hit: {statusEffect.amountPerHit}";
                            tooltipPoison.style.display = DisplayStyle.Flex;
                        }
                    }
                }

                tooltipBlockAbsorption.Q<Label>().text = $"Block Absorption: {weapon.blockAbsorption}%";
                tooltipBlockAbsorption.style.display = DisplayStyle.Flex;
            }

            if (item is Shield shield)
            {
                tooltipBlockAbsorption.Q<Label>().text = $"% {shield.defenseAbsorption} Damage Absorption";
                tooltipBlockAbsorption.style.display = DisplayStyle.Flex;

                if (shield.speedPenalty > 0)
                {
                    tooltipSpeedPenalty.Q<Label>().text = $"+ {Math.Round(shield.speedPenalty * 100, 2)}% Equip Load";
                    tooltipSpeedPenalty.style.display = DisplayStyle.Flex;
                    tooltipSpeedPenalty.style.opacity = 1;
                }
            }

            if (item is ArmorBase armor)
            {
                tooltipPhysicalDefense.Q<Label>().text = $"+ {armor.physicalDefense} Physical Defense";
                tooltipPhysicalDefense.style.display = armor.physicalDefense > 0 ? DisplayStyle.Flex : DisplayStyle.None;

                if (armor.speedPenalty > 0)
                {
                    tooltipSpeedPenalty.Q<Label>().text = $"+ {Math.Round(armor.speedPenalty * 100, 2)}% Equip Load";
                    tooltipSpeedPenalty.style.display = DisplayStyle.Flex;
                    tooltipSpeedPenalty.style.opacity = 1;
                }
                if (armor.fireDefense > 0)
                {
                    tooltipFire.Q<Label>().text = $"+ {armor.fireDefense} Fire Defense";
                    tooltipFire.style.display = DisplayStyle.Flex;
                }
                if (armor.frostDefense > 0)
                {
                    tooltipFrost.Q<Label>().text = $"+ {armor.frostDefense} Frost Defense";
                    tooltipFrost.style.display = DisplayStyle.Flex;
                }
                if (armor.lightningDefense > 0)
                {
                    tooltipLightning.Q<Label>().text = $"+ {armor.lightningDefense} Lightning Defense";
                    tooltipLightning.style.display = DisplayStyle.Flex;
                }
                if (armor.magicDefense > 0)
                {
                    tooltipMagic.Q<Label>().text = $"+ {armor.magicDefense} Magic Defense";
                    tooltipMagic.style.display = DisplayStyle.Flex;
                }

                if (armor.poiseBonus > 0)
                {
                    tooltipPoise.Q<Label>().text = $"+ {armor.poiseBonus} Poise";
                    tooltipPoise.style.display = DisplayStyle.Flex;
                }

                if (armor.statusEffectResistances != null && armor.statusEffectResistances.Length > 0)
                {
                    foreach (var statusEffect in armor.statusEffectResistances)
                    {
                        if (statusEffect.statusEffect.name == "Bleed")
                        {
                            tooltipBleed.Q<Label>().text = $"Bleed Resistance: {statusEffect.resistanceBonus}";
                            tooltipBleed.style.display = DisplayStyle.Flex;
                        }

                        if (statusEffect.statusEffect.name == "Poison")
                        {
                            tooltipPoison.Q<Label>().text = $"Poison Resistance: {statusEffect.resistanceBonus}";
                            tooltipPoison.style.display = DisplayStyle.Flex;
                        }
                    }
                }

                if (armor.additionalCoinPercentage != 0)
                {
                    tooltipGold.Q<Label>().text = $"+ %{armor.additionalCoinPercentage} gold found on enemies";
                    tooltipGold.style.display = DisplayStyle.Flex;
                }

                if (armor.vitalityBonus != 0)
                {
                    tooltipVitality.Q<Label>().text = $"+ {armor.vitalityBonus} Vitality";
                    tooltipVitality.style.display = DisplayStyle.Flex;
                }
                if (armor.enduranceBonus != 0)
                {
                    tooltipEndurance.Q<Label>().text = $"+ {armor.enduranceBonus} Endurance";
                    tooltipEndurance.style.display = DisplayStyle.Flex;
                }
                if (armor.strengthBonus != 0)
                {
                    tooltipStrength.Q<Label>().text = $"+ {armor.strengthBonus} Strength";
                    tooltipStrength.style.display = DisplayStyle.Flex;
                }
                if (armor.dexterityBonus != 0)
                {
                    tooltipDexterity.Q<Label>().text = $"+ {armor.dexterityBonus} Dexterity";
                    tooltipDexterity.style.display = DisplayStyle.Flex;
                }
                if (armor.intelligenceBonus != 0)
                {
                    tooltipIntelligence.Q<Label>().text = $"+ {armor.intelligenceBonus} Intelligence";
                    tooltipIntelligence.style.display = DisplayStyle.Flex;
                }
                if (armor.reputationBonus != 0)
                {
                    tooltipReputationBonus.Q<Label>().text = (armor.reputationBonus > 0 ? "+" : "") + armor.reputationBonus + " Reputation";
                    tooltipReputationBonus.style.display = DisplayStyle.Flex;
                }
            }

            if (item is Accessory accessory)
            {
                tooltipAccessoryProperty.Q<Label>().text = accessory.shortDescription.GetEnglishText();
                tooltipAccessoryProperty.style.display = DisplayStyle.Flex;
            }

            if (item is Consumable consumable && item is not ConsumableProjectile)
            {
                tooltipConsumableEffect.Q<Label>().text = consumable.shortDescription.GetEnglishText();
                tooltipConsumableEffect.style.display = DisplayStyle.Flex;
            }

        }

        public void DisplayTooltip(Button parentButton)
        {
            // Get the button's position and size in screen space
            float buttonY = parentButton.worldBound.y;
            float buttonWidth = parentButton.resolvedStyle.width;

            // Get the tooltip's size
            float tooltipSize = tooltip.resolvedStyle.height;

            // Calculate the target position for the tooltip
            Vector2 tooltipPosition = new Vector2(buttonWidth, buttonY / 2);

            // Check if the tooltip would exceed the screen height
            float screenHeight = root.resolvedStyle.height;
            if (tooltipPosition.y + tooltipSize > screenHeight)
            {
                // Adjust the position to be above the button if it would be outside the screen
                float tooltipOffset = tooltipSize;
                tooltipPosition.y = Mathf.Max(buttonY - tooltipOffset, 0f);
            }

            // Position the tooltip
            tooltip.style.display = DisplayStyle.Flex;
        }
    }
}
