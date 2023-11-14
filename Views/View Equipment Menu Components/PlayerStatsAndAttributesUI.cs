using System;
using System.Collections.Generic;
using System.Linq;
using AF.Stats;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF.UI.EquipmentMenu
{
    public class PlayerStatsAndAttributesUI : MonoBehaviour
    {
        // Footer Stats Display
        public const string LEVEL = "Level";
        public const string VITALITY = "Vitality";
        public const string ENDURANCE = "Endurance";
        public const string STRENGTH = "Strength";
        public const string DEXTERITY = "Dexterity";
        public const string INTELLIGENCE = "Intelligence";
        public const string ATTACK = "Attack";
        public const string DEFENSE = "Defense";
        public const string DEFENSE_FIRE = "DefenseFire";
        public const string DEFENSE_FROST = "DefenseFrost";
        public const string DEFENSE_LIGHTNING = "DefenseLightning";
        public const string DEFENSE_MAGIC = "DefenseMagic";
        public const string POISE = "Poise";
        public const string REPUTATION = "Reputation";
        public const string GOLD = "Gold";
        public const string EQUIP_LOAD = "EquipLoad";

        Dictionary<string, Label> statsAndAttributesLabels = new();

        [Header("Components")]
        public StatsBonusController playerStatsBonusController;
        public EquipmentGraphicsHandler equipmentGraphicsHandler;
        public AttackStatManager attackStatManager;
        public DefenseStatManager defenseStatManager;
        public PlayerPoiseController playerPoiseController;

        [Header("UI Documents")]
        public UIDocument uIDocument;
        public VisualElement root;


        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;

        [HideInInspector] public bool shouldRerender = true;

        private void OnEnable()
        {
            if (shouldRerender)
            {
                shouldRerender = false;

                SetupRefs();
            }
        }

        void SetupRefs()
        {
            root = uIDocument.rootVisualElement;

            var labelNames = new[] { LEVEL, VITALITY, ENDURANCE, STRENGTH, DEXTERITY, INTELLIGENCE,
                        ATTACK, DEFENSE, DEFENSE_FIRE, DEFENSE_FROST, DEFENSE_LIGHTNING, DEFENSE_MAGIC, POISE, EQUIP_LOAD, REPUTATION, GOLD
                }.ToList();

            var attributesContainer = root.Q<VisualElement>("Footer");
            foreach (var labelName in labelNames)
            {
                statsAndAttributesLabels[labelName] = attributesContainer.Q<Label>(labelName);
            }
        }

        public void DrawStats(Item item)
        {
            statsAndAttributesLabels[LEVEL].text = "Level " + playerStatsDatabase.GetCurrentLevel();
            statsAndAttributesLabels[GOLD].text = "Gold " + playerStatsDatabase.gold;

            int baseAttack = equipmentDatabase.GetCurrentWeapon() != null
                ? attackStatManager.GetWeaponAttack(equipmentDatabase.GetCurrentWeapon())
                : attackStatManager.GetCurrentPhysicalAttack();

            int itemAttack = 0;
            if (item is Weapon weapon)
            {
                itemAttack = (int)attackStatManager.GetWeaponAttack(weapon);
            }
            else if (item is Accessory accessory && equipmentDatabase.IsAccessoryEquiped(accessory))
            {
                itemAttack = baseAttack + accessory.physicalAttackBonus;
            }

            UpdateStatLabel(
                statsAndAttributesLabels[ATTACK],
                "Attack ",
                baseAttack,
                itemAttack);

            int baseVitality = playerStatsDatabase.vitality + playerStatsBonusController.vitalityBonus;
            int baseEndurance = playerStatsDatabase.endurance + playerStatsBonusController.enduranceBonus;
            int baseStrength = playerStatsDatabase.strength + playerStatsBonusController.strengthBonus;
            int baseDexterity = playerStatsDatabase.dexterity + playerStatsBonusController.dexterityBonus;
            int baseIntelligence = playerStatsDatabase.intelligence + playerStatsBonusController.intelligenceBonus;
            int baseReputation = playerStatsDatabase.GetCurrentReputation();
            int vitalityFromItem = 0;
            int enduranceFromItem = 0;
            int strengthFromItem = 0;
            int dexterityFromItem = 0;
            int reputationFromItem = 0;
            int intelligenceFromItem = 0;

            int basePhysicalDefense = (int)defenseStatManager.GetDefenseAbsorption();
            int itemPhysicalDefense = 0;

            int baseFireDefense = (int)defenseStatManager.GetFireDefense();
            int baseFrostDefense = (int)defenseStatManager.GetFrostDefense();
            int baseLightningDefense = (int)defenseStatManager.GetLightningDefense();
            int baseMagicDefense = (int)defenseStatManager.GetMagicDefense();
            int itemFireDefense = -1;
            int itemFrostDefense = -1;
            int itemLightningDefense = -1;
            int itemMagicDefense = -1;
            int basePoise = playerPoiseController.GetMaxPoise();
            int itemPoise = 0;

            float baseEquipLoad = equipmentGraphicsHandler.GetEquipLoad();
            float itemEquipLoad = -1f;


            if (item is ArmorBase armorBase)
            {
                // EDGE CASDE: If is not accessory that is already equipped
                if (armorBase is Accessory acc && equipmentDatabase.IsAccessoryEquiped(acc) == false)
                {
                    itemPhysicalDefense = GetElementalDefenseFromItem(armorBase, WeaponElementType.None);
                    itemFireDefense = GetElementalDefenseFromItem(armorBase, WeaponElementType.Fire);
                    itemFrostDefense = GetElementalDefenseFromItem(armorBase, WeaponElementType.Frost);
                    itemLightningDefense = GetElementalDefenseFromItem(armorBase, WeaponElementType.Lightning);
                    itemMagicDefense = GetElementalDefenseFromItem(armorBase, WeaponElementType.Magic);
                }


                itemEquipLoad = GetEquipLoadFromItem(armorBase);

                itemPoise = GetPoiseFromItem(armorBase);
                vitalityFromItem = GetAttributeFromEquipment(armorBase, AttributeType.VITALITY);
                enduranceFromItem = GetAttributeFromEquipment(armorBase, AttributeType.ENDURANCE);
                strengthFromItem = GetAttributeFromEquipment(armorBase, AttributeType.STRENGTH);
                dexterityFromItem = GetAttributeFromEquipment(armorBase, AttributeType.DEXTERITY);
                reputationFromItem = GetAttributeFromEquipment(armorBase, AttributeType.REPUTATION);
                intelligenceFromItem = GetAttributeFromEquipment(armorBase, AttributeType.INTELLIGENCE);
            }
            else if (item is Weapon weaponItem)
            {
                itemEquipLoad = GetEquipLoadFromItem(weaponItem);
            }
            else if (item is Shield shield)
            {
                itemEquipLoad = GetEquipLoadFromItem(shield);
            }

            UpdateStatLabel(
                statsAndAttributesLabels[VITALITY],
                "Vitality ",
                baseVitality,
                vitalityFromItem);

            UpdateStatLabel(
                statsAndAttributesLabels[ENDURANCE],
                "Endurance ",
                baseEndurance,
                enduranceFromItem);

            UpdateStatLabel(
                statsAndAttributesLabels[STRENGTH],
                "Strength ",
                baseStrength,
                strengthFromItem);

            UpdateStatLabel(
                statsAndAttributesLabels[DEXTERITY],
                "Dexterity ",
                baseDexterity,
                dexterityFromItem);

            UpdateStatLabel(
                statsAndAttributesLabels[INTELLIGENCE],
                "Intelligence ",
                baseIntelligence,
                intelligenceFromItem);

            UpdateStatLabel(
                statsAndAttributesLabels[REPUTATION],
                "Reputation ",
                baseReputation,
                reputationFromItem);

            UpdateStatLabel(
                statsAndAttributesLabels[DEFENSE],
                "Defense ",
                basePhysicalDefense,
                itemPhysicalDefense);

            UpdateElementalStatLabel(
                statsAndAttributesLabels[DEFENSE_FIRE],
                baseFireDefense,
                itemFireDefense);

            UpdateElementalStatLabel(
                statsAndAttributesLabels[DEFENSE_FROST],
                baseFrostDefense,
                itemFrostDefense);

            UpdateElementalStatLabel(
                statsAndAttributesLabels[DEFENSE_LIGHTNING],
                baseLightningDefense,
                itemLightningDefense);

            UpdateElementalStatLabel(
                statsAndAttributesLabels[DEFENSE_MAGIC],
                baseMagicDefense,
                itemMagicDefense);

            UpdateStatLabel(
                statsAndAttributesLabels[POISE],
                "Poise ",
                basePoise,
                itemPoise);

            UpdateEquipLoadStatLabel(
                statsAndAttributesLabels[EQUIP_LOAD],
                "Weight Load ",
                baseEquipLoad,
                itemEquipLoad);
        }

        void UpdateStatLabel(Label label, string labelPrefix, int leftValue, int rightValue)
        {
            label.text = labelPrefix;

            if (rightValue != 0)
            {
                label.text += rightValue;

                if (rightValue > leftValue)
                {
                    label.style.color = Color.green;
                }
                else if (rightValue < leftValue)
                {
                    label.style.color = Color.red;
                }

                return;
            }

            label.text += leftValue;
            label.style.color = Color.white;
        }

        void UpdateElementalStatLabel(Label label, int leftValue, int rightValue)
        {
            label.text = "";
            label.style.display = DisplayStyle.None;

            // No new equipment
            if (rightValue == -1)
            {

                if (leftValue > 0)
                {
                    label.text += leftValue;
                    label.style.display = DisplayStyle.Flex;
                }

                return;
            }

            // New equipment has no elemental property
            if (rightValue == 0)
            {
                label.style.display = DisplayStyle.None;
                return;
            }

            // New equipment has elemental property
            label.text += rightValue;
            label.style.display = DisplayStyle.Flex;
        }


        void UpdateEquipLoadStatLabel(Label label, string labelPrefix, float leftValue, float rightValue)
        {
            label.text = labelPrefix;

            var suffix = "";

            if (rightValue >= 0)
            {
                if (equipmentGraphicsHandler.IsLightWeightForGivenValue(rightValue))
                {
                    suffix = " (Light)";
                }
                else if (equipmentGraphicsHandler.IsMidWeightForGivenValue(rightValue))
                {
                    suffix = " (Medium)";
                }
                else if (equipmentGraphicsHandler.IsHeavyWeightForGivenValue(rightValue))
                {
                    suffix = " (Heavy)";
                }

                label.text += Math.Round(rightValue * 100, 2) + "%" + suffix;


                if (rightValue > leftValue)
                {
                    label.style.color = Color.green;
                }
                else if (rightValue < leftValue)
                {
                    label.style.color = Color.red;
                }

                return;
            }

            if (equipmentGraphicsHandler.IsLightWeightForGivenValue(leftValue))
            {
                suffix = " (Light)";
            }
            else if (equipmentGraphicsHandler.IsMidWeightForGivenValue(leftValue))
            {
                suffix = " (Medium)";
            }
            else if (equipmentGraphicsHandler.IsHeavyWeightForGivenValue(leftValue))
            {
                suffix = " (Heavy)";
            }

            label.text += Math.Round(leftValue * 100, 2) + "%" + suffix;
            label.style.color = Color.white;
        }

        int GetElementalDefenseFromItem(ArmorBase armorBase, WeaponElementType weaponElementType)
        {
            int baseElementalDefense = GetBaseElementalDefense(weaponElementType);

            Dictionary<string, Dictionary<WeaponElementType, int>> equippedItemsDictionary = new()
            {
                { "AF.Helmet", CreateDefenseDictionary(equipmentDatabase.helmet) },
                { "AF.Armor", CreateDefenseDictionary(equipmentDatabase.armor) },
                { "AF.Gauntlet", CreateDefenseDictionary(equipmentDatabase.gauntlet) },
                { "AF.Legwear", CreateDefenseDictionary(equipmentDatabase.legwear) },
                { "AF.Accessory", CreateDefenseDictionary(null) }
            };

            int newValue = baseElementalDefense;

            if (equippedItemsDictionary.TryGetValue(armorBase.GetType().ToString(), out var equippedItemDefense))
            {
                if (equippedItemDefense.TryGetValue(weaponElementType, out var currentDefenseFromItem))
                {
                    newValue -= currentDefenseFromItem;
                    if (newValue < 0)
                    {
                        newValue = 0;
                    }

                    newValue += GetDefenseValueForEquipmentType(armorBase, weaponElementType);
                }
            }

            return newValue;
        }

        int GetBaseElementalDefense(WeaponElementType weaponElementType)
        {
            Dictionary<WeaponElementType, Func<float>> defenseDictionary = new()
            {
                { WeaponElementType.Fire, defenseStatManager.GetFireDefense },
                { WeaponElementType.Frost, defenseStatManager.GetFrostDefense },
                { WeaponElementType.Lightning, defenseStatManager.GetLightningDefense },
                { WeaponElementType.Magic, defenseStatManager.GetMagicDefense },
                { WeaponElementType.None, defenseStatManager.GetDefenseAbsorption },
            };

            return defenseDictionary.TryGetValue(weaponElementType, out var defenseFunction) ? (int)defenseFunction() : 0;
        }

        Dictionary<WeaponElementType, int> CreateDefenseDictionary(ArmorBase armor)
        {
            return new()
            {
                { WeaponElementType.Fire, GetDefenseValueForEquipmentType(armor, WeaponElementType.Fire) },
                { WeaponElementType.Frost, GetDefenseValueForEquipmentType(armor, WeaponElementType.Frost) },
                { WeaponElementType.Lightning, GetDefenseValueForEquipmentType(armor, WeaponElementType.Lightning) },
                { WeaponElementType.Magic, GetDefenseValueForEquipmentType(armor, WeaponElementType.Magic) },
                { WeaponElementType.None, GetDefenseValueForEquipmentType(armor, WeaponElementType.None) }
            };
        }

        int GetDefenseValueForEquipmentType(ArmorBase armorBase, WeaponElementType weaponElementType)
        {
            Dictionary<WeaponElementType, Func<ArmorBase, float>> defenseValueDictionary = new()
            {
                { WeaponElementType.Fire, armor => armor == null ? 0 : armor.fireDefense },
                { WeaponElementType.Frost, armor => armor == null ? 0 :armor.frostDefense },
                { WeaponElementType.Lightning, armor => armor == null ? 0 :armor.lightningDefense },
                { WeaponElementType.Magic, armor => armor == null ? 0 : armor.magicDefense },
                { WeaponElementType.None, armor => armor == null ? 0 :armor.physicalDefense }
            };

            if (defenseValueDictionary.ContainsKey(weaponElementType))
            {
                return (int)defenseValueDictionary[weaponElementType](armorBase);
            }

            return 0;
        }

        int GetPoiseFromItem(ArmorBase armorBase)
        {
            var currentPoise = playerPoiseController.GetMaxPoise();
            if (armorBase is Helmet helmet)
            {
                var poiseFromEquippedItem = equipmentDatabase.helmet != null ? equipmentDatabase.helmet.poiseBonus : 0;
                currentPoise -= poiseFromEquippedItem;
                if (currentPoise < 0) currentPoise = 0;

                return currentPoise + helmet.poiseBonus;
            }

            if (armorBase is Armor armor)
            {
                var poiseFromEquippedItem = equipmentDatabase.armor != null ? equipmentDatabase.armor.poiseBonus : 0;
                currentPoise -= poiseFromEquippedItem;
                if (currentPoise < 0) currentPoise = 0;

                return currentPoise + armor.poiseBonus;
            }

            if (armorBase is Gauntlet gauntlet)
            {
                var poiseFromEquippedItem = equipmentDatabase.gauntlet != null ? equipmentDatabase.gauntlet.poiseBonus : 0;
                currentPoise -= poiseFromEquippedItem;
                if (currentPoise < 0) currentPoise = 0;

                return currentPoise + gauntlet.poiseBonus;
            }

            if (armorBase is Legwear legwear)
            {
                var poiseFromEquippedItem = equipmentDatabase.legwear != null ? equipmentDatabase.legwear.poiseBonus : 0;
                currentPoise -= poiseFromEquippedItem;
                if (currentPoise < 0) currentPoise = 0;

                return currentPoise + legwear.poiseBonus;
            }

            if (armorBase is Accessory accessory)
            {
                var poiseBonus = 0;
                if (equipmentDatabase.IsAccessoryEquiped(accessory) == false)
                {
                    poiseBonus = accessory.poiseBonus;
                }

                return currentPoise + poiseBonus;
            }

            return 0;
        }


        float GetEquipLoadFromItem(Item armorBase)
        {
            float currentWeightPenalty = playerStatsBonusController.weightPenalty;

            if (armorBase is Weapon wp)
            {
                var speedPenaltyFromItem = equipmentDatabase.GetCurrentWeapon() != null ? equipmentDatabase.GetCurrentWeapon().speedPenalty : 0;
                currentWeightPenalty += speedPenaltyFromItem * -1f;
                if (currentWeightPenalty <= 0) currentWeightPenalty = 0;

                return currentWeightPenalty + wp.speedPenalty * 1f;
            }
            else if (armorBase is Shield shield)
            {
                var speedPenaltyFromItem = equipmentDatabase.GetCurrentShield() != null ? equipmentDatabase.GetCurrentShield().speedPenalty : 0;
                currentWeightPenalty += speedPenaltyFromItem * -1f;
                if (currentWeightPenalty <= 0) currentWeightPenalty = 0;

                return currentWeightPenalty + shield.speedPenalty * 1f;
            }
            else if (armorBase is Helmet helmet)
            {
                var speedPenaltyFromItem = equipmentDatabase.helmet != null ? equipmentDatabase.helmet.speedPenalty : 0;
                currentWeightPenalty += speedPenaltyFromItem * -1f;
                if (currentWeightPenalty <= 0) currentWeightPenalty = 0;

                return currentWeightPenalty + helmet.speedPenalty * 1f;
            }
            else if (armorBase is Armor armorItem)
            {
                var speedPenaltyFromItem = equipmentDatabase.armor != null ? equipmentDatabase.armor.speedPenalty : 0;
                currentWeightPenalty += speedPenaltyFromItem * -1f;
                if (currentWeightPenalty <= 0) currentWeightPenalty = 0;

                return currentWeightPenalty + armorItem.speedPenalty * 1f;
            }
            else if (armorBase is Gauntlet gauntlet)
            {
                var speedPenaltyFromItem = equipmentDatabase.gauntlet != null ? equipmentDatabase.gauntlet.speedPenalty : 0;
                currentWeightPenalty += speedPenaltyFromItem * -1f;
                if (currentWeightPenalty <= 0) currentWeightPenalty = 0;

                return currentWeightPenalty + gauntlet.speedPenalty * 1f;
            }
            else if (armorBase is Legwear legwear)
            {
                var speedPenaltyFromItem = equipmentDatabase.legwear != null ? equipmentDatabase.legwear.speedPenalty : 0;
                currentWeightPenalty += speedPenaltyFromItem * -1f;
                if (currentWeightPenalty <= 0) currentWeightPenalty = 0;

                return currentWeightPenalty + legwear.speedPenalty * 1f;
            }
            else if (armorBase is Accessory acc)
            {
                var speedPenaltyFromItem = equipmentDatabase.accessories.Sum(x => x != null ? x.speedPenalty : 0);

                currentWeightPenalty += speedPenaltyFromItem * -1f;
                if (currentWeightPenalty <= 0) currentWeightPenalty = 0;

                return currentWeightPenalty + acc.speedPenalty * 1f;
            }

            return 0f;
        }

        enum AttributeType { VITALITY, ENDURANCE, DEXTERITY, STRENGTH, INTELLIGENCE, REPUTATION };
        int GetAttributeFromEquipment(ArmorBase armorBase, AttributeType attributeType)
        {
            int currentValue = 0;
            int bonusFromEquipment = 0;

            if (attributeType == AttributeType.VITALITY)
            {
                currentValue = playerStatsDatabase.vitality + playerStatsBonusController.vitalityBonus;
                bonusFromEquipment = armorBase.vitalityBonus;
            }
            else if (attributeType == AttributeType.ENDURANCE)
            {
                currentValue = playerStatsDatabase.endurance + playerStatsBonusController.enduranceBonus;
                bonusFromEquipment = armorBase.enduranceBonus;
            }
            else if (attributeType == AttributeType.STRENGTH)
            {
                currentValue = playerStatsDatabase.strength + playerStatsBonusController.strengthBonus;
                bonusFromEquipment = armorBase.strengthBonus;
            }
            else if (attributeType == AttributeType.DEXTERITY)
            {
                currentValue = playerStatsDatabase.dexterity + playerStatsBonusController.dexterityBonus;
                bonusFromEquipment = armorBase.dexterityBonus;
            }
            else if (attributeType == AttributeType.INTELLIGENCE)
            {
                currentValue = playerStatsDatabase.intelligence + playerStatsBonusController.intelligenceBonus;
                bonusFromEquipment = armorBase.intelligenceBonus;
            }
            else if (attributeType == AttributeType.REPUTATION)
            {
                currentValue = playerStatsDatabase.GetCurrentReputation();
                bonusFromEquipment = armorBase.reputationBonus;
            }

            int valueFromCurrentEquipment = 0;
            if (armorBase is Helmet)
            {
                if (equipmentDatabase.helmet != null)
                {
                    Helmet equippedHelmet = equipmentDatabase.helmet;

                    if (attributeType == AttributeType.VITALITY) { valueFromCurrentEquipment = equippedHelmet.vitalityBonus; }
                    else if (attributeType == AttributeType.ENDURANCE) { valueFromCurrentEquipment = equippedHelmet.enduranceBonus; }
                    else if (attributeType == AttributeType.STRENGTH) { valueFromCurrentEquipment = equippedHelmet.strengthBonus; }
                    else if (attributeType == AttributeType.DEXTERITY) { valueFromCurrentEquipment = equippedHelmet.dexterityBonus; }
                    else if (attributeType == AttributeType.REPUTATION) { valueFromCurrentEquipment = equippedHelmet.reputationBonus; }
                    else if (attributeType == AttributeType.INTELLIGENCE) { valueFromCurrentEquipment = equippedHelmet.intelligenceBonus; }
                }
            }
            else if (armorBase is Armor)
            {
                if (equipmentDatabase.armor != null)
                {
                    Armor equippedArmor = equipmentDatabase.armor;

                    if (attributeType == AttributeType.VITALITY) { valueFromCurrentEquipment = equippedArmor.vitalityBonus; }
                    else if (attributeType == AttributeType.ENDURANCE) { valueFromCurrentEquipment = equippedArmor.enduranceBonus; }
                    else if (attributeType == AttributeType.STRENGTH) { valueFromCurrentEquipment = equippedArmor.strengthBonus; }
                    else if (attributeType == AttributeType.DEXTERITY) { valueFromCurrentEquipment = equippedArmor.dexterityBonus; }
                    else if (attributeType == AttributeType.REPUTATION) { valueFromCurrentEquipment = equippedArmor.reputationBonus; }
                    else if (attributeType == AttributeType.INTELLIGENCE) { valueFromCurrentEquipment = equippedArmor.intelligenceBonus; }
                }
            }
            else if (armorBase is Gauntlet)
            {
                if (equipmentDatabase.gauntlet != null)
                {
                    Gauntlet equippedGauntlet = equipmentDatabase.gauntlet;

                    if (attributeType == AttributeType.VITALITY) { valueFromCurrentEquipment = equippedGauntlet.vitalityBonus; }
                    else if (attributeType == AttributeType.ENDURANCE) { valueFromCurrentEquipment = equippedGauntlet.enduranceBonus; }
                    else if (attributeType == AttributeType.STRENGTH) { valueFromCurrentEquipment = equippedGauntlet.strengthBonus; }
                    else if (attributeType == AttributeType.DEXTERITY) { valueFromCurrentEquipment = equippedGauntlet.dexterityBonus; }
                    else if (attributeType == AttributeType.REPUTATION) { valueFromCurrentEquipment = equippedGauntlet.reputationBonus; }
                    else if (attributeType == AttributeType.INTELLIGENCE) { valueFromCurrentEquipment = equippedGauntlet.intelligenceBonus; }
                }
            }
            else if (armorBase is Legwear)
            {
                if (equipmentDatabase.legwear != null)
                {
                    Legwear equippedLegwear = equipmentDatabase.legwear;

                    if (attributeType == AttributeType.VITALITY) { valueFromCurrentEquipment = equippedLegwear.vitalityBonus; }
                    else if (attributeType == AttributeType.ENDURANCE) { valueFromCurrentEquipment = equippedLegwear.enduranceBonus; }
                    else if (attributeType == AttributeType.STRENGTH) { valueFromCurrentEquipment = equippedLegwear.strengthBonus; }
                    else if (attributeType == AttributeType.DEXTERITY) { valueFromCurrentEquipment = equippedLegwear.dexterityBonus; }
                    else if (attributeType == AttributeType.REPUTATION) { valueFromCurrentEquipment = equippedLegwear.reputationBonus; }
                    else if (attributeType == AttributeType.INTELLIGENCE) { valueFromCurrentEquipment = equippedLegwear.intelligenceBonus; }
                }
            }
            else if (armorBase is Accessory accessory)
            {
                if (equipmentDatabase.IsAccessoryEquiped(accessory))
                {
                    bonusFromEquipment = 0;
                }
            }

            currentValue -= valueFromCurrentEquipment;
            if (currentValue < 0) currentValue = 0;

            return currentValue + bonusFromEquipment;
        }
    }
}
