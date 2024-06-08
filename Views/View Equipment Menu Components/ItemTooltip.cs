using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF.UI.EquipmentMenu
{
    public class ItemTooltip : MonoBehaviour
    {
        public PlayerManager playerManager;

        public const string TOOLTIP = "ItemTooltip";
        VisualElement tooltip;
        VisualElement itemInfo;
        VisualElement tooltipItemSprite;
        Label tooltipItemDescription;

        VisualElement tooltipEffectsContainer;

        public VisualTreeAsset itemEffectTooltipEntry;

        [Header("Components")]
        public AttackStatManager attackStatManager;
        public RecipesDatabase recipesDatabase;

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
        public Color requirementsNotMet;


        public Texture2D weaponPhysicalAttackSprite, weaponScalingSprite, weightPenaltySprite, holyWeaponSprite,
        fireSprite, frostSprite, lightningSprite, magicSprite, darknessSprite, bluntSprite, pierceSprite, slashSprite,
        statusEffectsSprite, defenseAbsorptionSprite, poiseSprite, postureSprite, goldCoinSprite, reputationSprite, barterSprite,
        vitalitySprite, enduranceSprite, intelligenceSprite, strengthSprite, dexteritySprite, blacksmithSprite,
        pushForceSprite, heavyAttackSprite, staminaCostSprite, bossTokenSprite, replenishableSprite, spellCastSprite,
        upgradeMaterialSprite, craftingMaterialSprite, projectileSprite, requirementsSprite;

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
            if (weapon.HasRequirements())
            {
                CreateTooltip(
                    requirementsSprite,
                    weapon.AreRequirementsMet(playerManager.statsBonusController) ? Color.white : requirementsNotMet,
                    weapon.DrawRequirements(playerManager.statsBonusController));
            }

            var strengthAttackBonus = attackStatManager.GetStrengthBonusFromWeapon(weapon);
            var dexterityAttackBonus = attackStatManager.GetDexterityBonusFromWeapon(weapon);
            var intelligenceAttackBonus = attackStatManager.GetIntelligenceBonusFromWeapon(weapon);
            var unscaledAttackDamage = (int)(attackStatManager.GetWeaponAttack(weapon) - strengthAttackBonus - dexterityAttackBonus - intelligenceAttackBonus) - weapon.damage.physical;

            string damageExplanation = $"+{attackStatManager.GetWeaponAttack(weapon)} Final Damage\n\n";
            damageExplanation += $"Explanation: \n";
            damageExplanation += $"+{weapon.damage.physical} Weapon Base Damage\n";
            damageExplanation += $"+{strengthAttackBonus} ATK [STR Scaling: {weapon.strengthScaling}]\n";
            damageExplanation += $"+{dexterityAttackBonus} ATK [DEX Scaling: {weapon.dexterityScaling}]\n";
            damageExplanation += $"+{intelligenceAttackBonus} ATK [INT Scaling: {weapon.intelligenceScaling}]\n";
            //damageExplanation += $"+{unscaledAttackDamage} Unscaled Physical Damage";
            CreateTooltip(weaponPhysicalAttackSprite, Color.white, damageExplanation);

            CreateEquipLoadTooltip(weapon.speedPenalty);

            if (weapon.isHolyWeapon)
            {
                CreateTooltip(holyWeaponSprite, Color.white, "Holy Weapon");
            }
            if (weapon.GetWeaponFireAttack() > 0)
            {
                CreateTooltip(fireSprite, fire, $"+{weapon.GetWeaponFireAttack() * attackStatManager.GetIntelligenceBonusFromWeapon(weapon)} Fire ATK");
            }
            if (weapon.GetWeaponFrostAttack() > 0)
            {
                CreateTooltip(frostSprite, frost, $"+{weapon.GetWeaponFrostAttack() * attackStatManager.GetIntelligenceBonusFromWeapon(weapon)} Frost ATK");
            }
            if (weapon.GetWeaponLightningAttack() > 0)
            {
                CreateTooltip(lightningSprite, lightning, $"+{weapon.GetWeaponLightningAttack() * attackStatManager.GetIntelligenceBonusFromWeapon(weapon)} Lightning ATK");
            }
            if (weapon.GetWeaponMagicAttack() > 0)
            {
                CreateTooltip(magicSprite, magic, $"+{weapon.GetWeaponMagicAttack() * attackStatManager.GetIntelligenceBonusFromWeapon(weapon)} Magic ATK");
            }
            if (weapon.GetWeaponDarknessAttack() > 0)
            {
                CreateTooltip(darknessSprite, darkness, $"+{weapon.GetWeaponDarknessAttack() * attackStatManager.GetIntelligenceBonusFromWeapon(weapon)} Darkness ATK");
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

            if (weapon.blockAbsorption != 1)
            {
                CreateTooltip(defenseAbsorptionSprite, Color.white, $"%{100 - (weapon.blockAbsorption * 100)} Physical DMG Absorption when blocking");
            }

            if (weapon.doubleCoinsUponKillingEnemies)
            {
                CreateTooltip(goldCoinSprite, Color.white, "Double coins per enemy kill");
            }

            if (weapon.healthRestoredWithEachHit > 0)
            {
                CreateTooltip(vitalitySprite, Color.white, $"+{weapon.healthRestoredWithEachHit}HP restored with each hit");
            }
        }

        void DrawShield(Shield shield)
        {
            if (shield.blockStaminaCost != 1)
            {
                CreateTooltip(staminaCostSprite, Color.white, $"-{shield.blockStaminaCost} Stamina Cost Per Block");
            }
            if (shield.physicalAbsorption != 1)
            {
                CreateTooltip(defenseAbsorptionSprite, Color.white, $"%{100 - (shield.physicalAbsorption * 100)} Physical DMG Absorption");
            }
            if (shield.fireAbsorption != 1)
            {
                CreateTooltip(fireSprite, fire, $"%{100 - (shield.fireAbsorption * 100)} Fire DMG Absorption");
            }
            if (shield.frostAbsorption != 1)
            {
                CreateTooltip(frostSprite, frost, $"%{100 - (shield.frostAbsorption * 100)} Frost DMG Absorption");
            }
            if (shield.lightiningAbsorption != 1)
            {
                CreateTooltip(lightningSprite, lightning, $"%{100 - (shield.lightiningAbsorption * 100)} Lightning DMG Absorption");
            }
            if (shield.magicAbsorption != 1)
            {
                CreateTooltip(magicSprite, magic, $"%{100 - (shield.magicAbsorption * 100)} Magic DMG Absorption");
            }
            if (shield.darknessAbsorption != 1)
            {
                CreateTooltip(darknessSprite, darkness, $"%{100 - (shield.darknessAbsorption * 100)} Darkness DMG Absorption");
            }

            if (shield.statusEffectBlockResistances != null && shield.statusEffectBlockResistances.Length > 0)
            {
                CreateTooltip(statusEffectsSprite, Color.white, shield.GetFormattedStatusResistances());
            }

            if (shield.poiseBonus != 0)
            {
                CreatePoiseTooltip(shield.poiseBonus);
            }

            if (shield.postureBonus != 0)
            {
                CreatePostureTooltip(shield.postureBonus);
            }

            if (shield.postureDamageAbsorption != 1)
            {
                CreateTooltip(postureSprite, Color.white, $"{100 - shield.postureDamageAbsorption * 100}% Posture DMG Absorption");
            }
            if (shield.slashDamageAbsorption != 1)
            {
                CreateTooltip(slashSprite, Color.white, $"{100 - shield.slashDamageAbsorption * 100}% Slash DMG Absorption");
            }
            if (shield.pierceDamageAbsorption != 1)
            {
                CreateTooltip(pierceSprite, Color.white, $"{100 - shield.pierceDamageAbsorption * 100}% Pierce DMG Absorption");
            }
            if (shield.bluntDamageAbsorption != 1)
            {
                CreateTooltip(bluntSprite, Color.white, $"{100 - shield.bluntDamageAbsorption * 100}% Blunt DMG Absorption");
            }

            if (shield.canDamageEnemiesOnShieldAttack)
            {

                if (shield.damageDealtToEnemiesUponBlocking.physical != 0)
                {
                    CreateTooltip(weaponPhysicalAttackSprite, Color.white, $"{shield.damageDealtToEnemiesUponBlocking.physical} Physical DMG dealt to enemies per block");
                }

                if (shield.damageDealtToEnemiesUponBlocking.fire != 0)
                {
                    CreateTooltip(fireSprite, fire, $"{shield.damageDealtToEnemiesUponBlocking.fire} Fire DMG dealt to enemies per block");
                }

                if (shield.damageDealtToEnemiesUponBlocking.frost != 0)
                {
                    CreateTooltip(frostSprite, frost, $"{shield.damageDealtToEnemiesUponBlocking.frost} Frost DMG dealt to enemies per block");
                }

                if (shield.damageDealtToEnemiesUponBlocking.lightning != 0)
                {
                    CreateTooltip(lightningSprite, lightning, $"{shield.damageDealtToEnemiesUponBlocking.lightning} Lightning DMG dealt to enemies per block");
                }

                if (shield.damageDealtToEnemiesUponBlocking.magic != 0)
                {
                    CreateTooltip(magicSprite, magic, $"{shield.damageDealtToEnemiesUponBlocking.magic} Magic DMG dealt to enemies per block");
                }

                if (shield.damageDealtToEnemiesUponBlocking.darkness != 0)
                {
                    CreateTooltip(darknessSprite, darkness, $"{shield.damageDealtToEnemiesUponBlocking.darkness} Darkness DMG dealt to enemies per block");
                }

                if (shield.damageDealtToEnemiesUponBlocking.statusEffects != null && shield.damageDealtToEnemiesUponBlocking.statusEffects.Length > 0)
                {
                    CreateTooltip(statusEffectsSprite, Color.white, shield.GetFormattedStatusAttacks());
                }
            }

            if (shield.parryWindowBonus != 0)
            {
                CreateTooltip(defenseAbsorptionSprite, Color.white, $"+{shield.parryWindowBonus} Parry Window Duration Bonus");
            }

            if (shield.parryPostureDamageBonus != 0)
            {
                CreateTooltip(defenseAbsorptionSprite, Color.white, $"+{shield.parryPostureDamageBonus} Posture DMG per Parry");
            }

            if (shield.vitalityBonus != 0)
            {
                CreateTooltip(vitalitySprite, Color.white, $"+{shield.vitalityBonus} Vitality");
            }

            if (shield.enduranceBonus != 0)
            {
                CreateTooltip(enduranceSprite, Color.white, $"+{shield.enduranceBonus} Endurance");
            }

            if (shield.intelligenceBonus != 0)
            {
                CreateTooltip(intelligenceSprite, Color.white, $"+{shield.intelligenceBonus} Intelligence");
            }

            if (shield.staminaRegenBonus != 1)
            {
                CreateTooltip(staminaCostSprite, Color.white, $"%{shield.staminaRegenBonus} Stamina Regen. Speed Bonus");
            }

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

            if (armor.poiseBonus != 0)
            {
                CreatePoiseTooltip(armor.poiseBonus);
            }

            if (armor.postureBonus != 0)
            {
                CreatePostureTooltip(armor.postureBonus);
            }

            if (armor.statusEffectResistances != null && armor.statusEffectResistances.Length > 0)
            {
                CreateTooltip(statusEffectsSprite, Color.white, armor.GetFormattedStatusResistances());
            }

            CreateAdditionalGoldTooltip(armor.additionalCoinPercentage);

            CreateStatTooltip(armor.vitalityBonus, "Vitality", vitalitySprite);
            CreateStatTooltip(armor.enduranceBonus, "Endurance", enduranceSprite);
            CreateStatTooltip(armor.intelligenceBonus, "Intelligence", intelligenceSprite);
            CreateStatTooltip(armor.strengthBonus, "Strength", strengthSprite);
            CreateStatTooltip(armor.dexterityBonus, "Dexterity", dexteritySprite);

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

            if (armor.canDamageEnemiesUponAttack)
            {
                if (armor.damageDealtToEnemiesUponAttacked.physical != 0)
                {
                    CreateTooltip(weaponPhysicalAttackSprite, Color.white, $"{armor.damageDealtToEnemiesUponAttacked.physical} Physical DMG dealt to attacking enemies");
                }

                if (armor.damageDealtToEnemiesUponAttacked.fire != 0)
                {
                    CreateTooltip(fireSprite, fire, $"{armor.damageDealtToEnemiesUponAttacked.fire} Fire DMG dealt to attacking enemies");
                }

                if (armor.damageDealtToEnemiesUponAttacked.frost != 0)
                {
                    CreateTooltip(frostSprite, frost, $"{armor.damageDealtToEnemiesUponAttacked.frost} Frost DMG dealt to attacking enemies");
                }

                if (armor.damageDealtToEnemiesUponAttacked.lightning != 0)
                {
                    CreateTooltip(lightningSprite, lightning, $"{armor.damageDealtToEnemiesUponAttacked.lightning} Lightning DMG dealt to attacking enemies");
                }

                if (armor.damageDealtToEnemiesUponAttacked.magic != 0)
                {
                    CreateTooltip(magicSprite, magic, $"{armor.damageDealtToEnemiesUponAttacked.magic} Magic DMG dealt to attacking enemies");
                }

                if (armor.damageDealtToEnemiesUponAttacked.darkness != 0)
                {
                    CreateTooltip(darknessSprite, darkness, $"{armor.damageDealtToEnemiesUponAttacked.darkness} Darkness DMG dealt to attacking enemies");
                }

                if (armor.damageDealtToEnemiesUponAttacked.statusEffects != null && armor.damageDealtToEnemiesUponAttacked.statusEffects.Length > 0)
                {
                    CreateTooltip(statusEffectsSprite, Color.white, armor.GetFormattedDamageDealtToEnemiesUpponAttacked());
                }
            }

            if (armor.projectileMultiplierBonus > 0)
            {
                CreateTooltip(projectileSprite, Color.white, $"x{armor.projectileMultiplierBonus}% damage on projectiles");
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
            if (accessory.postureDecreaseRateBonus > 0)
            {
                CreateTooltip(postureSprite, Color.white, $"+{accessory.postureDecreaseRateBonus} Posture Decrease Rate Bonus");
            }
            if (accessory.backStabAngleBonus > 0)
            {
                CreateTooltip(postureSprite, Color.white, $"+{accessory.backStabAngleBonus} Backstab Angle Bonus");
            }
        }

        void DrawSpell(Spell spell)
        {
            if (spell.HasRequirements())
            {
                CreateTooltip(
                    requirementsSprite,
                    spell.AreRequirementsMet(playerManager.statsBonusController) ? Color.white : requirementsNotMet,
                    spell.DrawRequirements(playerManager.statsBonusController));
            }

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

            if (CraftingUtils.IsItemAnIngredientOfCurrentLearnedRecipes(recipesDatabase, craftingMaterial))
            {
                CraftingRecipe[] craftingRecipes = CraftingUtils.GetRecipesUsingItem(recipesDatabase, craftingMaterial).ToArray();
                if (craftingRecipes != null && craftingRecipes.Length > 0)
                {
                    CreateTooltip(craftingMaterialSprite, Color.white, CraftingUtils.GetFormattedTextForRecipesUsingItem(craftingRecipes));
                }
            }
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
