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

        VisualElement tooltipEffectsContainer;

        public VisualTreeAsset itemEffectTooltipEntry;

        [Header("Components")]
        public AttackStatManager attackStatManager;

        [Header("UI Documents")]
        public UIDocument uIDocument;
        public VisualElement root;

        [HideInInspector] public bool shouldRerender = true;

        [Header("Colors")]
        public Color fire = new Color(255, 102, 42);
        public Color frost = new Color(96, 202, 255);
        public Color lightning = new Color(255, 239, 95);
        public Color magic = new Color(241, 96, 255);
        public Color darkness = new Color(92, 76, 248);


        public Texture2D weaponPhysicalAttackSprite, weaponScalingSprite, weightPenaltySprite, holyWeaponSprite,
        fireSprite, frostSprite, lightningSprite, magicSprite, darknessSprite, bluntSprite, pierceSprite, slashSprite,
        statusEffectsSprite, defenseAbsorptionSprite, poiseSprite, postureSprite, goldCoinSprite, reputationSprite, barterSprite,
        vitalitySprite, enduranceSprite, intelligenceSprite, strengthSprite, dexteritySprite, blacksmithSprite,
        pushForceSprite, heavyAttackSprite, staminaCostSprite, bossTokenSprite, replenishableSprite, spellCastSprite,
        upgradeMaterialSprite, craftingMaterialSprite;

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
            tooltipEffectsContainer = tooltip.Q<VisualElement>("ItemAttributes");
        }

        public void PrepareTooltipForItem(Item item)
        {
            enabled = true;
            tooltipEffectsContainer.Clear();
            tooltipItemSprite.style.backgroundImage = new StyleBackground(item.sprite);

            string itemName = item.name.ToUpper();

            if (item is Weapon wp)
            {
                itemName += " +" + wp.level;
            }

            tooltipItemDescription.text = itemName + " \n" + '"' + item.itemDescription + '"';

            if (item is Weapon weapon)
            {
                DrawWeaponEffects(weapon);
            }
            else if (item is Shield shield)
            {
                DrawShield(shield);
            }
            else if (item is ArmorBase armorBase)
            {
                DrawArmorBase(armorBase);

                if (item is Accessory accessory)
                {
                    DrawAccessory(accessory);
                }
            }
            else if (item is Consumable consumable)
            {
                DrawConsumable(consumable);
            }
            else if (item is Spell spell)
            {
                DrawSpell(spell);
            }
            else if (item is UpgradeMaterial upgradeMaterial)
            {
                DrawUpgradeMaterial(upgradeMaterial);
            }
            else if (item is CraftingMaterial craftingMaterial)
            {
                DrawCraftingMaterial(craftingMaterial);
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

        void CreateEquipLoadTooltip(float speedPenalty)
        {
            if (speedPenalty <= 0)
            {
                return;
            }

            CreateTooltip(weightPenaltySprite, Color.white, $"+{Math.Round(speedPenalty * 100, 2)}% Equip Load");
        }

        void CreatePoiseTooltip(int poiseBonus)
        {
            if (poiseBonus <= 0)
            {
                return;
            }

            CreateTooltip(poiseSprite, Color.white, $"+{poiseBonus} Poise Points");
        }
        void CreatePostureTooltip(int postureBonus)
        {
            if (postureBonus <= 0)
            {
                return;
            }

            CreateTooltip(postureSprite, Color.white, $"+{postureBonus} Posture Points");
        }

        void CreateAdditionalGoldTooltip(float additionalCoinPercentage)
        {
            if (additionalCoinPercentage <= 0)
            {
                return;
            }
            CreateTooltip(goldCoinSprite, Color.white, $"+ {additionalCoinPercentage}% gold found on enemies");
        }

        void CreateStatTooltip(int statBonus, string statName, Texture2D statSprite)
        {
            if (statBonus != 0)
            {
                CreateTooltip(statSprite, Color.white, $"+{statBonus} {statName}");
            }
        }

        void DrawWeaponEffects(Weapon weapon)
        {
            var strengthAttackBonus = attackStatManager.GetStrengthBonusFromWeapon(weapon);
            var dexterityAttackBonus = attackStatManager.GetDexterityBonusFromWeapon(weapon);
            var intelligenceAttackBonus = attackStatManager.GetIntelligenceBonusFromWeapon(weapon);
            var unscaledAttackDamage = (int)(attackStatManager.GetWeaponAttack(weapon) - strengthAttackBonus - dexterityAttackBonus - intelligenceAttackBonus) - weapon.damage.physical;

            string damageExplanation = $"+{attackStatManager.GetWeaponAttack(weapon)} Final Damage\n";
            damageExplanation += $"Explanation: \n\n";
            damageExplanation += $"+{weapon.damage.physical} Weapon Base Damage\n";
            damageExplanation += $"+{strengthAttackBonus} ATK [STR Scaling: {weapon.strengthScaling}]\n";
            damageExplanation += $"+{dexterityAttackBonus} ATK [DEX Scaling: {weapon.dexterityScaling}]\n";
            damageExplanation += $"+{intelligenceAttackBonus} ATK [INT Scaling: {weapon.intelligenceScaling}]\n";
            damageExplanation += $"+{unscaledAttackDamage} Unscaled Physical Damage";
            CreateTooltip(weaponPhysicalAttackSprite, Color.white, damageExplanation);

            CreateEquipLoadTooltip(weapon.speedPenalty);

            if (weapon.isHolyWeapon)
            {
                CreateTooltip(holyWeaponSprite, Color.white, "Holy Weapon");
            }
            if (weapon.GetWeaponFireAttack() > 0)
            {
                CreateTooltip(fireSprite, fire, $"+{weapon.GetWeaponFireAttack()} Fire ATK");
            }
            if (weapon.GetWeaponFrostAttack() > 0)
            {
                CreateTooltip(frostSprite, frost, $"+{weapon.GetWeaponFrostAttack()} Frost ATK");
            }
            if (weapon.GetWeaponLightningAttack() > 0)
            {
                CreateTooltip(lightningSprite, lightning, $"+{weapon.GetWeaponLightningAttack()} Lightning ATK");
            }
            if (weapon.GetWeaponMagicAttack() > 0)
            {
                CreateTooltip(magicSprite, magic, $"+{weapon.GetWeaponMagicAttack()} Magic ATK");
            }
            if (weapon.GetWeaponDarknessAttack() > 0)
            {
                CreateTooltip(darknessSprite, darkness, $"+{weapon.GetWeaponDarknessAttack()} Darkness ATK");
            }
            if (weapon.damage.weaponAttackType == WeaponAttackType.Blunt)
            {
                CreateTooltip(bluntSprite, Color.white, $"Damage Type: Blunt");
            }
            if (weapon.damage.weaponAttackType == WeaponAttackType.Pierce)
            {
                CreateTooltip(pierceSprite, Color.white, $"Damage Type: Pierce");
            }
            if (weapon.damage.weaponAttackType == WeaponAttackType.Slash)
            {
                CreateTooltip(slashSprite, Color.white, $"Damage Type: Slash");
            }
            if (weapon.damage.statusEffects != null && weapon.damage.statusEffects.Length > 0)
            {
                CreateTooltip(statusEffectsSprite, Color.white, weapon.GetFormattedStatusDamages());
            }

            if (weapon.damage.pushForce > 0)
            {
                CreateTooltip(pushForceSprite, Color.white, $"+{weapon.damage.pushForce} Push Force");
            }

            if (weapon.damage.postureDamage > 0)
            {
                CreateTooltip(postureSprite, Color.white, $"+{weapon.damage.postureDamage} Posture DMG");
            }

            if (weapon.heavyAttackBonus > 0)
            {
                CreateTooltip(heavyAttackSprite, Color.white, $"+{weapon.heavyAttackBonus} Heavy ATK Bonus");
            }

            CreateTooltip(staminaCostSprite, Color.white, $"{weapon.lightAttackStaminaCost} Light ATK Stamina Cost\n{weapon.heavyAttackStaminaCost} Heavy ATK Stamina Cost");

            if (weapon.canBeUpgraded && weapon.CanBeUpgradedFurther())
            {
                CreateTooltip(blacksmithSprite, Color.white, weapon.GetMaterialCostForNextLevel());
            }

            if (weapon.damage.ignoreBlocking)
            {
                CreateTooltip(defenseAbsorptionSprite, Color.white, "Ignores enemy shields");
            }

            if (weapon.damage.canNotBeParried)
            {
                CreateTooltip(defenseAbsorptionSprite, Color.white, "Can not be parried");
            }
        }

        void DrawShield(Shield shield)
        {
            CreateTooltip(defenseAbsorptionSprite, Color.white, $"%{shield.defenseAbsorption} Damage Absorption");
            CreateEquipLoadTooltip(shield.speedPenalty);
        }

        void DrawArmorBase(ArmorBase armor)
        {
            if (armor.physicalDefense > 0)
            {
                CreateTooltip(defenseAbsorptionSprite, Color.white, $"+{armor.physicalDefense} Physical DEF");
            }

            if (armor.speedPenalty > 0)
            {
                CreateEquipLoadTooltip(armor.speedPenalty);
            }

            if (armor.fireDefense > 0)
            {
                CreateTooltip(fireSprite, fire, $"+{armor.fireDefense} Fire DEF");
            }
            if (armor.frostDefense > 0)
            {
                CreateTooltip(frostSprite, frost, $"+{armor.frostDefense} Frost DEF");
            }
            if (armor.lightningDefense > 0)
            {
                CreateTooltip(lightningSprite, lightning, $"+{armor.lightningDefense} Lightning DEF");
            }
            if (armor.magicDefense > 0)
            {
                CreateTooltip(magicSprite, magic, $"+{armor.magicDefense} Magic DEF");
            }
            if (armor.darkDefense > 0)
            {
                CreateTooltip(darknessSprite, darkness, $"+{armor.darkDefense} Darkness DEF");
            }

            CreatePoiseTooltip(armor.poiseBonus);
            CreatePostureTooltip(armor.postureBonus);

            if (armor.statusEffectResistances != null && armor.statusEffectResistances.Length > 0)
            {
                CreateTooltip(statusEffectsSprite, Color.white, armor.GetFormattedStatusResistances());
            }

            CreateAdditionalGoldTooltip(armor.additionalCoinPercentage);

            CreateStatTooltip(armor.vitalityBonus, "Vitality", vitalitySprite);
            CreateStatTooltip(armor.enduranceBonus, "Vitality", enduranceSprite);
            CreateStatTooltip(armor.intelligenceBonus, "Vitality", intelligenceSprite);
            CreateStatTooltip(armor.strengthBonus, "Vitality", strengthSprite);
            CreateStatTooltip(armor.dexterityBonus, "Vitality", dexteritySprite);

            if (armor.reputationBonus > 0)
            {
                CreateTooltip(reputationSprite, Color.white, $"+{armor.reputationBonus} Reputation");

            }
            else if (armor.reputationBonus < 0)
            {
                CreateTooltip(reputationSprite, Color.white, $"-{armor.reputationBonus} Reputation");

            }

            if (armor.discountPercentage > 0)
            {
                CreateTooltip(barterSprite, Color.white, $"+{Math.Round(armor.discountPercentage * 100, 2)}% Better Prices");
            }

        }

        void DrawAccessory(Accessory accessory)
        {
            if (accessory.shortDescription != null && accessory.shortDescription.Length > 0)
            {
                CreateTooltip(statusEffectsSprite, Color.white, accessory.shortDescription);
            }

            if (accessory.healthBonus > 0)
            {
                CreateTooltip(vitalitySprite, Color.white, $"+{accessory.healthBonus} Health Points");
            }
            if (accessory.magicBonus > 0)
            {
                CreateTooltip(magicSprite, Color.white, $"+{accessory.magicBonus} Mana Points");
            }
            if (accessory.staminaBonus > 0)
            {
                CreateTooltip(enduranceSprite, Color.white, $"+{accessory.staminaBonus} Stamina Points");
            }
            if (accessory.physicalAttackBonus > 0)
            {
                CreateTooltip(weaponPhysicalAttackSprite, Color.white, $"+{accessory.physicalAttackBonus} Physical ATK");
            }
            if (accessory.jumpAttackBonus > 0)
            {
                CreateTooltip(weaponPhysicalAttackSprite, Color.white, $"+{accessory.jumpAttackBonus} Jump ATK Bonus");
            }
            if (accessory.increaseAttackPowerWithLowerHealth)
            {
                CreateTooltip(weaponPhysicalAttackSprite, Color.white, $"ATK increases with lower health");
            }
            if (accessory.increaseAttackPowerTheLowerTheReputation)
            {
                CreateTooltip(weaponPhysicalAttackSprite, Color.white, $"ATK increases with lower reputation");
            }
            if (accessory.postureDamagePerParry > 0)
            {
                CreateTooltip(postureSprite, Color.white, $"+{accessory.postureDamagePerParry} Posture Damage per Parry");
            }
            if (accessory.spellDamageBonusMultiplier > 0)
            {
                CreateTooltip(magicSprite, Color.white, $"+{Math.Round(accessory.spellDamageBonusMultiplier * 100, 2)}% Spell Damage");
            }
            if (accessory.chanceToDoubleCoinsFromFallenEnemies)
            {
                CreateTooltip(goldCoinSprite, Color.white, $"Chance to receive double coins from fallen enemies");
            }
        }

        void DrawSpell(Spell spell)
        {
            if (spell.shortDescription != null && spell.shortDescription.Length > 0)
            {
                CreateTooltip(statusEffectsSprite, Color.white, spell.shortDescription);
            }
            if (spell.costPerCast > 0)
            {
                CreateTooltip(spellCastSprite, Color.white, $"{spell.costPerCast} Mana Points required per cast");
            }

            if (spell.statusEffects != null && spell.statusEffects.Length > 0)
            {
                CreateTooltip(statusEffectsSprite, Color.white, spell.GetFormattedAppliedStatusEffects());
            }

            /*            if (spell.isFaithSpell)
                        {
                            CreateTooltip(holyWeaponSprite, Color.white, $"Faith Spell (improves with reputation)");
                        }*/

        }

        void DrawCraftingMaterial(CraftingMaterial craftingMaterial)
        {
            if (craftingMaterial.shortDescription != null && craftingMaterial.shortDescription.Length > 0)
            {
                CreateTooltip(statusEffectsSprite, Color.white, craftingMaterial.shortDescription);
            }

            CreateTooltip(craftingMaterialSprite, Color.white, "Crafting material (Use in a alchemy table)");
        }

        void DrawUpgradeMaterial(UpgradeMaterial upgradeMaterial)
        {
            if (upgradeMaterial.shortDescription != null && upgradeMaterial.shortDescription.Length > 0)
            {
                CreateTooltip(statusEffectsSprite, Color.white, upgradeMaterial.shortDescription);
            }

            CreateTooltip(upgradeMaterialSprite, Color.white, "Weapon upgrade material (Give to a blacksmith)");
        }

        void DrawConsumable(Consumable consumable)
        {
            if (consumable.shortDescription != null && consumable.shortDescription.Length > 0)
            {
                CreateTooltip(statusEffectsSprite, Color.white, consumable.shortDescription);
            }
            if (consumable.statusesToRemove != null && consumable.statusesToRemove.Length > 0)
            {
                CreateTooltip(statusEffectsSprite, Color.white, consumable.GetFormattedRemovedStatusEffects());
            }
            if (consumable.statusesToRemove != null && consumable.statusEffectsWhenConsumed.Length > 0)
            {
                CreateTooltip(statusEffectsSprite, Color.white, consumable.GetFormattedAppliedStatusEffects());
            }
            if (consumable.isBossToken)
            {
                CreateTooltip(bossTokenSprite, Color.white, $"Boss token. Someone might be interested in this item.");
            }
            if (consumable.canBeConsumedForGold)
            {
                CreateTooltip(goldCoinSprite, Color.white, $"Consume to receive ${consumable.value}");
            }
            if (consumable.shouldNotRemoveOnUse)
            {
                CreateTooltip(replenishableSprite, Color.white, "Item usage replenishes when resting at a bonfire");
            }

        }
        public void CreateTooltip(Texture2D sprite, Color color, string description)
        {
            VisualElement clone = itemEffectTooltipEntry.CloneTree();

            VisualElement icon = clone.Q<VisualElement>("Icon");
            icon.style.backgroundImage = new StyleBackground(sprite);
            icon.style.unityBackgroundImageTintColor = color;
            icon.style.borderTopColor = color;
            icon.style.borderLeftColor = color;
            icon.style.borderRightColor = color;
            icon.style.borderBottomColor = color;

            Label text = clone.Q<Label>();
            text.text = description;
            text.style.color = color;

            tooltipEffectsContainer.Add(clone);
        }


    }
}
