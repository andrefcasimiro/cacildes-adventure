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
            bool isEnglish = GamePreferences.instance.IsEnglish();

            statsAndAttributesLabels[LEVEL].text = (isEnglish ? "Level " : "Nível ") + playerStatsDatabase.GetCurrentLevel();
            statsAndAttributesLabels[GOLD].text = (isEnglish ? "Gold " : "Ouro ") + Player.instance.currentGold;

            int baseAttack = Player.instance.equippedWeapon != null
                ? attackStatManager.GetWeaponAttack(Player.instance.equippedWeapon)
                : attackStatManager.GetCurrentPhysicalAttack();

            int itemAttack = 0;
            if (item is Weapon weapon)
            {
                itemAttack = (int)attackStatManager.GetWeaponAttack(weapon);
            }
            else if (item is Accessory accessory && Player.instance.IsAccessoryEquiped(accessory) == false)
            {
                itemAttack = baseAttack + accessory.physicalAttackBonus;
            }

            UpdateStatLabel(
                statsAndAttributesLabels[ATTACK],
                isEnglish ? "Attack " : "Ataque ",
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
                if ((armorBase is Accessory acc && Player.instance.IsAccessoryEquiped(acc)) == false)
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
                isEnglish ? "Vitality " : "Vitalidade ",
                baseVitality,
                vitalityFromItem);

            UpdateStatLabel(
                statsAndAttributesLabels[ENDURANCE],
                isEnglish ? "Endurance " : "Resistência ",
                baseEndurance,
                enduranceFromItem);

            UpdateStatLabel(
                statsAndAttributesLabels[STRENGTH],
                isEnglish ? "Strength " : "Força ",
                baseStrength,
                strengthFromItem);

            UpdateStatLabel(
                statsAndAttributesLabels[DEXTERITY],
                isEnglish ? "Dexterity " : "Destreza ",
                baseDexterity,
                dexterityFromItem);

            UpdateStatLabel(
                statsAndAttributesLabels[INTELLIGENCE],
                isEnglish ? "Intelligence " : "Inteligência ",
                baseIntelligence,
                intelligenceFromItem);

            UpdateStatLabel(
                statsAndAttributesLabels[REPUTATION],
                isEnglish ? "Reputation " : "Reputação ",
                baseReputation,
                reputationFromItem);

            UpdateStatLabel(
                statsAndAttributesLabels[DEFENSE],
                isEnglish ? "Defense " : "Defesa ",
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
                isEnglish ? "Poise " : "Postura ",
                basePoise,
                itemPoise);

            UpdateEquipLoadStatLabel(
                statsAndAttributesLabels[EQUIP_LOAD],
                isEnglish ? "Weight Load " : "Peso ",
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
                    suffix = GamePreferences.instance.IsEnglish() ? " (Light)" : " (Leve)";
                }
                else if (equipmentGraphicsHandler.IsMidWeightForGivenValue(rightValue))
                {
                    suffix = GamePreferences.instance.IsEnglish() ? " (Medium)" : " (Médio)";
                }
                else if (equipmentGraphicsHandler.IsHeavyWeightForGivenValue(rightValue))
                {
                    suffix = GamePreferences.instance.IsEnglish() ? " (Heavy)" : " (Pesado)";
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
                suffix = GamePreferences.instance.IsEnglish() ? " (Light)" : " (Leve)";
            }
            else if (equipmentGraphicsHandler.IsMidWeightForGivenValue(leftValue))
            {
                suffix = GamePreferences.instance.IsEnglish() ? " (Medium)" : " (Médio)";
            }
            else if (equipmentGraphicsHandler.IsHeavyWeightForGivenValue(leftValue))
            {
                suffix = GamePreferences.instance.IsEnglish() ? " (Heavy)" : " (Pesado)";
            }

            label.text += Math.Round(leftValue * 100, 2) + "%" + suffix;
            label.style.color = Color.white;
        }

        int GetElementalDefenseFromItem(ArmorBase armorBase, WeaponElementType weaponElementType)
        {
            int baseElementalDefense = GetBaseElementalDefense(weaponElementType);

            Dictionary<string, Dictionary<WeaponElementType, int>> equippedItemsDictionary = new()
            {
                { "AF.Helmet", CreateDefenseDictionary(Player.instance.equippedHelmet) },
                { "AF.Armor", CreateDefenseDictionary(Player.instance.equippedArmor) },
                { "AF.Gauntlet", CreateDefenseDictionary(Player.instance.equippedGauntlets) },
                { "AF.Legwear", CreateDefenseDictionary(Player.instance.equippedLegwear) },
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
                var poiseFromEquippedItem = Player.instance.equippedHelmet != null ? Player.instance.equippedHelmet.poiseBonus : 0;
                currentPoise -= poiseFromEquippedItem;
                if (currentPoise < 0) currentPoise = 0;

                return currentPoise + helmet.poiseBonus;
            }

            if (armorBase is Armor armor)
            {
                var poiseFromEquippedItem = Player.instance.equippedArmor != null ? Player.instance.equippedArmor.poiseBonus : 0;
                currentPoise -= poiseFromEquippedItem;
                if (currentPoise < 0) currentPoise = 0;

                return currentPoise + armor.poiseBonus;
            }

            if (armorBase is Gauntlet gauntlet)
            {
                var poiseFromEquippedItem = Player.instance.equippedGauntlets != null ? Player.instance.equippedGauntlets.poiseBonus : 0;
                currentPoise -= poiseFromEquippedItem;
                if (currentPoise < 0) currentPoise = 0;

                return currentPoise + gauntlet.poiseBonus;
            }

            if (armorBase is Legwear legwear)
            {
                var poiseFromEquippedItem = Player.instance.equippedLegwear != null ? Player.instance.equippedLegwear.poiseBonus : 0;
                currentPoise -= poiseFromEquippedItem;
                if (currentPoise < 0) currentPoise = 0;

                return currentPoise + legwear.poiseBonus;
            }

            if (armorBase is Accessory accessory)
            {
                var poiseBonus = 0;
                if (Player.instance.IsAccessoryEquiped(accessory) == false)
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
                var speedPenaltyFromItem = Player.instance.equippedWeapon != null ? Player.instance.equippedWeapon.speedPenalty : 0;
                currentWeightPenalty += speedPenaltyFromItem * -1f;
                if (currentWeightPenalty <= 0) currentWeightPenalty = 0;

                return currentWeightPenalty + wp.speedPenalty * 1f;
            }
            else if (armorBase is Shield shield)
            {
                var speedPenaltyFromItem = Player.instance.equippedShield != null ? Player.instance.equippedShield.speedPenalty : 0;
                currentWeightPenalty += speedPenaltyFromItem * -1f;
                if (currentWeightPenalty <= 0) currentWeightPenalty = 0;

                return currentWeightPenalty + shield.speedPenalty * 1f;
            }
            else if (armorBase is Helmet helmet)
            {
                var speedPenaltyFromItem = Player.instance.equippedHelmet != null ? Player.instance.equippedHelmet.speedPenalty : 0;
                currentWeightPenalty += speedPenaltyFromItem * -1f;
                if (currentWeightPenalty <= 0) currentWeightPenalty = 0;

                return currentWeightPenalty + helmet.speedPenalty * 1f;
            }
            else if (armorBase is Armor armorItem)
            {
                var speedPenaltyFromItem = Player.instance.equippedArmor != null ? Player.instance.equippedArmor.speedPenalty : 0;
                currentWeightPenalty += speedPenaltyFromItem * -1f;
                if (currentWeightPenalty <= 0) currentWeightPenalty = 0;

                return currentWeightPenalty + armorItem.speedPenalty * 1f;
            }
            else if (armorBase is Gauntlet gauntlet)
            {
                var speedPenaltyFromItem = Player.instance.equippedGauntlets != null ? Player.instance.equippedGauntlets.speedPenalty : 0;
                currentWeightPenalty += speedPenaltyFromItem * -1f;
                if (currentWeightPenalty <= 0) currentWeightPenalty = 0;

                return currentWeightPenalty + gauntlet.speedPenalty * 1f;
            }
            else if (armorBase is Legwear legwear)
            {
                var speedPenaltyFromItem = Player.instance.equippedLegwear != null ? Player.instance.equippedLegwear.speedPenalty : 0;
                currentWeightPenalty += speedPenaltyFromItem * -1f;
                if (currentWeightPenalty <= 0) currentWeightPenalty = 0;

                return currentWeightPenalty + legwear.speedPenalty * 1f;
            }
            else if (armorBase is Accessory acc)
            {
                var speedPenaltyFromItem = Player.instance.equippedAccessories.Count > 0 ? Player.instance.equippedAccessories.Sum(x => x.speedPenalty) : 0;

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
                if (Player.instance.equippedHelmet != null)
                {

                    if (attributeType == AttributeType.VITALITY) { valueFromCurrentEquipment = Player.instance.equippedHelmet.vitalityBonus; }
                    else if (attributeType == AttributeType.ENDURANCE) { valueFromCurrentEquipment = Player.instance.equippedHelmet.enduranceBonus; }
                    else if (attributeType == AttributeType.STRENGTH) { valueFromCurrentEquipment = Player.instance.equippedHelmet.strengthBonus; }
                    else if (attributeType == AttributeType.DEXTERITY) { valueFromCurrentEquipment = Player.instance.equippedHelmet.dexterityBonus; }
                    else if (attributeType == AttributeType.REPUTATION) { valueFromCurrentEquipment = Player.instance.equippedHelmet.reputationBonus; }
                    else if (attributeType == AttributeType.INTELLIGENCE) { valueFromCurrentEquipment = Player.instance.equippedHelmet.intelligenceBonus; }
                }
            }
            else if (armorBase is Armor)
            {
                if (Player.instance.equippedArmor != null)
                {

                    if (attributeType == AttributeType.VITALITY) { valueFromCurrentEquipment = Player.instance.equippedArmor.vitalityBonus; }
                    else if (attributeType == AttributeType.ENDURANCE) { valueFromCurrentEquipment = Player.instance.equippedArmor.enduranceBonus; }
                    else if (attributeType == AttributeType.STRENGTH) { valueFromCurrentEquipment = Player.instance.equippedArmor.strengthBonus; }
                    else if (attributeType == AttributeType.DEXTERITY) { valueFromCurrentEquipment = Player.instance.equippedArmor.dexterityBonus; }
                    else if (attributeType == AttributeType.REPUTATION) { valueFromCurrentEquipment = Player.instance.equippedArmor.reputationBonus; }
                    else if (attributeType == AttributeType.INTELLIGENCE) { valueFromCurrentEquipment = Player.instance.equippedArmor.intelligenceBonus; }
                }
            }
            else if (armorBase is Gauntlet)
            {
                if (Player.instance.equippedGauntlets != null)
                {

                    if (attributeType == AttributeType.VITALITY) { valueFromCurrentEquipment = Player.instance.equippedGauntlets.vitalityBonus; }
                    else if (attributeType == AttributeType.ENDURANCE) { valueFromCurrentEquipment = Player.instance.equippedGauntlets.enduranceBonus; }
                    else if (attributeType == AttributeType.STRENGTH) { valueFromCurrentEquipment = Player.instance.equippedGauntlets.strengthBonus; }
                    else if (attributeType == AttributeType.DEXTERITY) { valueFromCurrentEquipment = Player.instance.equippedGauntlets.dexterityBonus; }
                    else if (attributeType == AttributeType.REPUTATION) { valueFromCurrentEquipment = Player.instance.equippedGauntlets.reputationBonus; }
                    else if (attributeType == AttributeType.INTELLIGENCE) { valueFromCurrentEquipment = Player.instance.equippedGauntlets.intelligenceBonus; }
                }
            }
            else if (armorBase is Legwear)
            {
                if (Player.instance.equippedLegwear != null)
                {

                    if (attributeType == AttributeType.VITALITY) { valueFromCurrentEquipment = Player.instance.equippedLegwear.vitalityBonus; }
                    else if (attributeType == AttributeType.ENDURANCE) { valueFromCurrentEquipment = Player.instance.equippedLegwear.enduranceBonus; }
                    else if (attributeType == AttributeType.STRENGTH) { valueFromCurrentEquipment = Player.instance.equippedLegwear.strengthBonus; }
                    else if (attributeType == AttributeType.DEXTERITY) { valueFromCurrentEquipment = Player.instance.equippedLegwear.dexterityBonus; }
                    else if (attributeType == AttributeType.REPUTATION) { valueFromCurrentEquipment = Player.instance.equippedLegwear.reputationBonus; }
                    else if (attributeType == AttributeType.INTELLIGENCE) { valueFromCurrentEquipment = Player.instance.equippedLegwear.intelligenceBonus; }
                }
            }
            else if (armorBase is Accessory accessory)
            {
                if (Player.instance.IsAccessoryEquiped(accessory))
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
