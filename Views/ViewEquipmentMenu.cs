using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public enum EquipmentMenuTab
    {
        WEAPON,
        SHIELD,
        ARROW,
        SPELL,
        HELMET,
        ARMOR,
        GAUNTLET,
        BOOTS,
        ACCESSORIES,
        CONSUMABLES,
        OTHER_ITEMS,
    }

    public class ViewEquipmentMenu : ViewMenu
    {

        #region Constants
        // Equipment Type Selection
        public const string WEAPON_BTN = "WeaponButton";
        public const string SHIELD_BTN = "ShieldButton";
        public const string ARROWS_BTN = "ArrowsButton";
        public const string SPELL_BTN = "SpellsButton";
        public const string HELMET_BTN = "HelmetButton";
        public const string ARMOR_BTN = "ArmorButton";
        public const string GAUNTLETS_BTN = "GauntletsButton";
        public const string BOOTS_BTN = "BootsButton";
        public const string ACCESSORIES_BTN = "AccessoriesButton";
        public const string CONSUMABLES_BTN = "ConsumablesButton";
        public const string OTHER_ITEMS_BTN = "OtherItemsButton";
        public const string SCROLL_ITEMS_LIST = "ItemsList";
        Button weaponBtn, shieldBtn, arrowsBtn, spellsBtn, helmetBtn, armorBtn, gauntletsBtn,
            bootsBtn, accessoriesBtn, consumablesBtn, otherItemsBtn;

        // Tooltip Logic
        public const string TOOLTIP = "ItemTooltip";
        VisualElement tooltip;
        VisualElement itemInfo;
        VisualElement tooltipItemSprite;
        Label tooltipItemDescription;
        VisualElement[] tooltipItemInfoChildren;
        // Common
        VisualElement tooltipSpeedPenalty;
        VisualElement tooltipPoise;
        VisualElement tooltipFire, tooltipFrost, tooltipLightning, tooltipMagic, tooltipPoison, tooltipBleed;
        VisualElement tooltipBlockAbsorption;
        // Weapons
        VisualElement tooltipPhysicalAttack;
        VisualElement tooltipWeaponType;
        VisualElement tooltipWeaponSpecial;
        VisualElement tooltipWeaponStrengthScaling, tooltipWeaponDexterityScaling;
        // Armor
        VisualElement tooltipPhysicalDefense;
        VisualElement tooltipAccessoryProperty;
        VisualElement tooltipReputationBonus, tooltipVitality, tooltipEndurance, tooltipStrength, tooltipDexterity, tooltipIntelligence, tooltipGold;
        // Consumables
        VisualElement tooltipConsumableEffect;

        Label menuLabel;


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

        Dictionary<string, Label> statsAndAttributesLabels = new();


        ScrollView itemsScrollView;

        private Button[] menuButtons;

        #endregion

        public VisualTreeAsset itemButtonPrefab;

        private EquipmentMenuTab activeTab = EquipmentMenuTab.WEAPON;
        private Button activeButton;

        AttackStatManager attackStatManager;
        EquipmentGraphicsHandler equipmentGraphicsHandler;
        PlayerLevelManager playerLevelManager;
        DefenseStatManager defenseStatManager;
        PlayerPoiseController playerPoiseController;
        FavoriteItemsManager favoriteItemsManager;

        // FIRE COLOR: FF662A
        // FROST COLOR: 60CAFF
        // LIGHTNING COLOR: FFEF5F
        // MAGIC COLOR: F160FF
        // BLEED COLOR: FF0910
        // POISON COLOR: AB44FF

        protected override void OnEnable()
        {
            base.OnEnable();

            SetupRefs();

            RedrawUI();
        }

        #region Refs
        void SetupRefs()
        {
            #region Equipment Type Selection Logic
            weaponBtn = root.Q<Button>(WEAPON_BTN);
            shieldBtn = root.Q<Button>(SHIELD_BTN);
            arrowsBtn = root.Q<Button>(ARROWS_BTN);
            spellsBtn = root.Q<Button>(SPELL_BTN);
            helmetBtn = root.Q<Button>(HELMET_BTN);
            armorBtn = root.Q<Button>(ARMOR_BTN);
            gauntletsBtn = root.Q<Button>(GAUNTLETS_BTN);
            bootsBtn = root.Q<Button>(BOOTS_BTN);
            accessoriesBtn = root.Q<Button>(ACCESSORIES_BTN);
            consumablesBtn = root.Q<Button>(CONSUMABLES_BTN);
            otherItemsBtn = root.Q<Button>(OTHER_ITEMS_BTN);

            menuButtons = new Button[]
                {
                    weaponBtn, shieldBtn, arrowsBtn, spellsBtn, helmetBtn, armorBtn,
                    gauntletsBtn, bootsBtn, accessoriesBtn, consumablesBtn, otherItemsBtn
                };



            if (activeButton == null)
            {
                activeButton = menuButtons[0];
            }

            foreach (var button in menuButtons)
            {
                button.clicked += () => {
                    this.activeTab = (EquipmentMenuTab)Array.IndexOf(menuButtons, button);
                    activeButton = button;

                    RedrawUI();
                };
            }


            #endregion

            #region Tooltip Logic
            tooltip = root.Q<VisualElement>(TOOLTIP);
            itemInfo = tooltip.Q<VisualElement>("ItemInfo");
            tooltipItemSprite = itemInfo.Q<VisualElement>("ItemSprite");
            tooltipItemDescription = itemInfo.Q<Label>();
            tooltipItemInfoChildren = tooltip.Q<VisualElement>("ItemAttributes").Children().ToArray();
            tooltipPhysicalAttack = tooltip.Q("PhysicalAttack");
            tooltipPhysicalDefense = tooltip.Q("PhysicalDefense");
            tooltipPhysicalDefense = tooltip.Q("PhysicalDefense");
            tooltipSpeedPenalty = tooltip.Q("SpeedPenalty");
            tooltipAccessoryProperty = tooltip.Q("AccessoryProperty");
            tooltipConsumableEffect = tooltip.Q("ConsumableEffect");
            tooltipWeaponType = tooltip.Q("WeaponType");
            tooltipWeaponSpecial = tooltip.Q("WeaponSpecial");
            tooltipWeaponStrengthScaling = tooltip.Q("StrengthScaling");
            tooltipWeaponDexterityScaling = tooltip.Q("DexterityScaling");
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

            #endregion

            menuLabel = root.Q<Label>("MenuLabel");

            #region Stats Logic

            var labelNames = new[] { LEVEL, VITALITY, ENDURANCE, STRENGTH, DEXTERITY, INTELLIGENCE,
                        ATTACK, DEFENSE, DEFENSE_FIRE, DEFENSE_FROST, DEFENSE_LIGHTNING, DEFENSE_MAGIC, POISE, REPUTATION, GOLD
                }.ToList();

            var attributesContainer = root.Q<VisualElement>("Footer");
            foreach (var labelName in labelNames)
            {
                statsAndAttributesLabels[labelName] = attributesContainer.Q<Label>(labelName);
            }
            #endregion

            itemsScrollView = root.Q<ScrollView>(SCROLL_ITEMS_LIST);

            #region Player Refs
            if (attackStatManager == null)
            {
                attackStatManager = FindAnyObjectByType<AttackStatManager>(FindObjectsInactive.Include);
                equipmentGraphicsHandler = attackStatManager.GetComponent<EquipmentGraphicsHandler>();
                playerLevelManager = attackStatManager.GetComponent<PlayerLevelManager>();
                defenseStatManager = attackStatManager.GetComponent<DefenseStatManager>();
                playerPoiseController = attackStatManager.GetComponent<PlayerPoiseController>();
                favoriteItemsManager = attackStatManager.GetComponent<FavoriteItemsManager>();
            }
            #endregion
        }
        #endregion

        void RedrawUI()
        {
            foreach (var button in menuButtons)
            {
                button.RemoveFromClassList("active");

                if (activeButton != null && button.tooltip == activeButton.tooltip) {
                    button.AddToClassList("active");
                }
            }

            DisplayMenuLabel(activeTab);

            HideTooltip();

            DrawItemsList();

            DrawStats(null);
        }

        void DisplayMenuLabel(EquipmentMenuTab tabToPreview)
        {
            bool isEnglish = GamePreferences.instance.IsEnglish();

            if (tabToPreview == EquipmentMenuTab.WEAPON)
            {
                menuLabel.text = isEnglish ? "Weapons" : "Armas";
            }
            else if (tabToPreview == EquipmentMenuTab.SHIELD)
            {
                menuLabel.text = isEnglish ? "Shields" : "Escudos";
            }
            else if (tabToPreview == EquipmentMenuTab.ARROW)
            {
                menuLabel.text = isEnglish ? "Arrows / Projectiles" : "Flechas & Projéteis";
            }
            else if (tabToPreview == EquipmentMenuTab.SPELL)
            {
                menuLabel.text = isEnglish ? "Spells" : "Feitiços";
            }
            else if (tabToPreview == EquipmentMenuTab.HELMET)
            {
                menuLabel.text = isEnglish ? "Helmets" : "Elmos";
            }
            else if (tabToPreview == EquipmentMenuTab.ARMOR)
            {
                menuLabel.text = isEnglish ? "Armors" : "Armaduras";
            }
            else if (tabToPreview == EquipmentMenuTab.GAUNTLET)
            {
                menuLabel.text = isEnglish ? "Gauntlets" : "Manoplas";
            }
            else if (tabToPreview == EquipmentMenuTab.BOOTS)
            {
                menuLabel.text = isEnglish ? "Legwear" : "Calçado";
            }
            else if (tabToPreview == EquipmentMenuTab.ACCESSORIES)
            {
                var currentEquipped = Player.instance.equippedAccessories.Count;
                var maxAllowed = equipmentGraphicsHandler.BASE_NUMBER_OF_ACCESSORIES_THAT_CAN_EQUIP + equipmentGraphicsHandler.GetExtraAccessorySlots();
                menuLabel.text = (isEnglish ? "Accessories " : "Acessórios ") + $"{currentEquipped}/{maxAllowed}";
            }
            else if (tabToPreview == EquipmentMenuTab.CONSUMABLES)
            {
                menuLabel.text = isEnglish ? "Consumables & Equipable Items" : "Consumíveis & Itens Equipáveis";
            }
            else if (tabToPreview == EquipmentMenuTab.OTHER_ITEMS)
            {
                menuLabel.text = isEnglish ? "All Items" : "Todos os itens";
            }
        }

        #region Populate Scroll View Logic
        public void DrawItemsList()
        {
            if (activeTab == EquipmentMenuTab.WEAPON)
            {
                PopulateScrollView<Weapon>();
            }
            else if (activeTab == EquipmentMenuTab.SHIELD)
            {
                PopulateScrollView<Shield>();
            }
            else if (activeTab == EquipmentMenuTab.ARROW)
            {
                PopulateScrollView<ConsumableProjectile>();
            }
            else if (activeTab == EquipmentMenuTab.SPELL)
            {
                PopulateScrollView<Spell>();
            }
            else if (activeTab == EquipmentMenuTab.HELMET)
            {
                PopulateScrollView<Helmet>();
            }
            else if (activeTab == EquipmentMenuTab.ARMOR)
            {
                PopulateScrollView<Armor>();
            }
            else if (activeTab == EquipmentMenuTab.GAUNTLET)
            {
                PopulateScrollView<Gauntlet>();
            }
            else if (activeTab == EquipmentMenuTab.BOOTS)
            {
                PopulateScrollView<Legwear>();
            }
            else if (activeTab == EquipmentMenuTab.ACCESSORIES)
            {
                PopulateScrollView<Accessory>();
            }
            else if (activeTab == EquipmentMenuTab.CONSUMABLES)
            {
                PopulateScrollView<Consumable>();
            }
            else if (activeTab == EquipmentMenuTab.OTHER_ITEMS)
            {
                PopulateScrollView<Item>();
            }
        }

        void PopulateScrollView<T>() where T : Item
        {
            this.itemsScrollView.Clear();

            bool isEnglish = GamePreferences.instance.IsEnglish();

            bool isWeapon = typeof(T) == typeof(Weapon);
            bool isShield = typeof(T) == typeof(Shield);
            bool isHelmet = typeof(T) == typeof(Helmet);
            bool isArmor = typeof(T) == typeof(Armor);
            bool isGauntlet = typeof(T) == typeof(Gauntlet);
            bool isLegwear = typeof(T) == typeof(Legwear);
            bool isAccessory = typeof(T) == typeof(Accessory);
            if (isWeapon || isShield || isHelmet || isArmor || isGauntlet || isLegwear || isAccessory)
            {
                var instance = itemButtonPrefab.CloneTree();
                instance.Q<VisualElement>("Sprite").style.display = DisplayStyle.None;
                instance.Q<Label>("ItemName").text = isEnglish ? "Unequip" : "Desequipar";
                instance.Q<Button>().clicked += () =>
                {
                    if (isWeapon)
                    {
                        equipmentGraphicsHandler.UnequipWeapon();
                    }
                    else if (isShield)
                    {
                        equipmentGraphicsHandler.UnequipShield();
                    }
                    else if (isHelmet)
                    {
                        equipmentGraphicsHandler.UnequipHelmet();
                    }
                    else if(isArmor)
                    {
                        equipmentGraphicsHandler.UnequipArmor();
                    }
                    else if (isGauntlet)
                    {
                        equipmentGraphicsHandler.UnequipGauntlet();
                    }
                    else if (isLegwear)
                    {
                        equipmentGraphicsHandler.UnequipLegwear();
                    }
                    else if (isAccessory)
                    {
                        var copy = Player.instance.equippedAccessories.ToList();
                        foreach (var acc in copy)
                        {
                            equipmentGraphicsHandler.OnUnequipAccessoryCheckIfAccessoryWasDestroyedPermanently(acc);
                        }

                        instance.Q<Label>("ItemName").text += " " + (isEnglish ? " All" : "Tudo");
                    }

                    RedrawUI();
                };

                instance.Q<VisualElement>("Indicator").style.display = DisplayStyle.None;
                instance.Q("Favorite").style.display = DisplayStyle.None;
                this.itemsScrollView.Add(instance);
            }


            foreach (var item in Player.instance.ownedItems)
            {
                if (item.item is not T typedItem)
                {
                    continue;
                }

                var instance = itemButtonPrefab.CloneTree();
                instance.Q<VisualElement>("Sprite").style.backgroundImage = new StyleBackground(item.item.sprite);
                var itemName = instance.Q<Label>("ItemName");
                itemName.text = item.item.name.GetText();

                #region Equipped Label Indicator
                bool itemIsEquipped = false;

                if (item.item is Weapon weapon)
                {
                    itemIsEquipped = Player.instance.equippedWeapon != null && Player.instance.equippedWeapon == weapon;

                    if (weapon.level > 1)
                    {
                        itemName.text += " +" + weapon.level;

                    }

                }
                else if (item.item is Shield shield)
                {
                    itemIsEquipped = Player.instance.equippedShield != null && Player.instance.equippedShield == shield;
                }
                else if (item.item is Helmet helmet)
                {
                    itemIsEquipped = Player.instance.equippedHelmet != null && Player.instance.equippedHelmet == helmet;
                }
                else if (item.item is Armor armor)
                {
                    itemIsEquipped = Player.instance.equippedArmor != null && Player.instance.equippedArmor == armor;
                }
                else if (item.item is Gauntlet gauntlet)
                {
                    itemIsEquipped = Player.instance.equippedGauntlets != null && Player.instance.equippedGauntlets == gauntlet;
                }
                else if (item.item is Legwear legwear)
                {
                    itemIsEquipped = Player.instance.equippedLegwear != null && Player.instance.equippedLegwear == legwear;
                }
                else if (item.item is Accessory accessory)
                {
                    itemIsEquipped = Player.instance.IsAccessoryEquiped(accessory);
                }
                else
                {
                    itemName.text += " (" + item.amount + ")";
                }

                if (itemIsEquipped)
                {
                    instance.style.opacity = 1.5f;
                    itemName.text += isEnglish ? " (Equipped)" : " (Equipado)";
                }
                #endregion

                #region Equipment Indicator
                var equipmentColorIndicator = GetEquipmentColorIndicator(item.item);
                if (equipmentColorIndicator == Color.black)
                {
                    instance.Q<VisualElement>("Indicator").style.display = DisplayStyle.None;
                }
                else
                {
                    instance.Q<VisualElement>("Indicator").style.backgroundColor = GetEquipmentColorIndicator(item.item);
                    instance.Q<VisualElement>("Indicator").style.display = DisplayStyle.Flex;
                }
                #endregion

                #region onClick
                var btn = instance.Q<Button>();
                btn.clicked += () =>
                {
                    if (item.item is Weapon weapon)
                    {
                        if (Player.instance.equippedWeapon == weapon)
                        {
                            equipmentGraphicsHandler.UnequipWeapon();
                        }
                        else
                        {
                            equipmentGraphicsHandler.EquipWeapon(weapon);
                        }

                    }
                    else if (item.item is Shield shield)
                    {
                        if (Player.instance.equippedShield == shield)
                        {
                            equipmentGraphicsHandler.UnequipShield();
                        }
                        else
                        {
                            equipmentGraphicsHandler.EquipShield(shield);
                        }

                    }
                    else if (item.item is Helmet helmet)
                    {
                        if (Player.instance.equippedHelmet == helmet)
                        {
                            equipmentGraphicsHandler.UnequipHelmet();
                        }
                        else
                        {
                            equipmentGraphicsHandler.EquipHelmet(helmet);
                        }

                    }
                    else if (item.item is Armor armor)
                    {
                        if (Player.instance.equippedArmor == armor)
                        {
                            equipmentGraphicsHandler.UnequipArmor();
                        }
                        else
                        {
                            equipmentGraphicsHandler.EquipArmor(armor);
                        }
                    }
                    else if (item.item is Gauntlet gauntlet)
                    {
                        if (Player.instance.equippedGauntlets == gauntlet)
                        {
                            equipmentGraphicsHandler.UnequipGauntlet();
                        }
                        else
                        {
                            equipmentGraphicsHandler.EquipGauntlet(gauntlet);
                        }
                    }
                    else if (item.item is Legwear legwear)
                    {
                        if (Player.instance.equippedLegwear == legwear)
                        {
                            equipmentGraphicsHandler.UnequipLegwear();
                        }
                        else
                        {
                            equipmentGraphicsHandler.EquipLegwear(legwear);
                        }

                    }
                    else if (item.item is Accessory accessory)
                    {
                        if (Player.instance.GetEquippedAccessoryIndex(accessory) != -1)
                        {
                            equipmentGraphicsHandler.OnUnequipAccessoryCheckIfAccessoryWasDestroyedPermanently(accessory);
                        }
                        else
                        {
                            equipmentGraphicsHandler.EquipAccessory(accessory);
                        }

                    }
                    else if (item.item is Consumable || item.item is Spell)
                    {
                        if (favoriteItemsManager.IsItemFavorited(item.item))
                        {
                            favoriteItemsManager.RemoveFavoriteItemFromList(item.item);
                        }
                        else
                        {
                            favoriteItemsManager.AddFavoriteItemToList(item.item);
                        }
                    }

                    RedrawUI();
                };
                #endregion

                #region On Focus
                instance.RegisterCallback<MouseEnterEvent>(ev =>
                {
                    PrepareTooltipForItem(item.item);
                    DisplayTooltip(btn);

                    DrawStats(item.item);
                });
                instance.RegisterCallback<MouseOutEvent>(ev =>
                {
                    HideTooltip();

                    DrawStats(null);
                });
                #endregion

                #region Favorite Logic
                if (favoriteItemsManager.IsItemFavorited(item.item))
                {
                    instance.Q("Favorite").style.display = DisplayStyle.Flex;
                }
                else
                {
                    instance.Q("Favorite").style.display = DisplayStyle.None;
                }

                #endregion


                if (!itemIsEquipped && isAccessory && equipmentGraphicsHandler.CanEquipMoreAccessories())
                {
                    instance.SetEnabled(false);
                }


                this.itemsScrollView.Add(instance);
            }
        }
        #endregion

        #region Tooltip Logic

        void PrepareTooltipForItem(Item item)
        {
            foreach (var child in tooltipItemInfoChildren)
            {
                child.style.display = DisplayStyle.None;
            }

            tooltipItemSprite.style.backgroundImage = new StyleBackground(item.sprite);
            tooltipItemDescription.text = item.name.GetEnglishText().ToUpper() + " \n" + '"' + item.description.GetEnglishText() + '"';

            bool isEnglish = GamePreferences.instance.IsEnglish();

            #region Weapon Tooltip Logic

            if (item is Weapon weapon)
            {

                var attack = attackStatManager.GetWeaponAttack(weapon);
                var strengthAttackBonus = attackStatManager.GetStrengthBonusFromWeapon(weapon);
                var dexterityAttackBonus = attackStatManager.GetDexterityBonusFromWeapon(weapon);
                attack = (int)(attack - strengthAttackBonus - dexterityAttackBonus);

                if (weapon.halveDamage) {
                    attack = (int)(attack / 2);
                }
                
                tooltipPhysicalAttack.Q<Label>().text = $"+ {attack} {(isEnglish ? "Physical Attack" : "Ataque Físico")}";
                
                if (weapon.halveDamage) {
                    tooltipPhysicalAttack.Q<Label>().text += " " + (isEnglish ? "(Halved damage)" : "(Dano divido)");
                }


                tooltipPhysicalAttack.style.display = DisplayStyle.Flex;

                if (weapon.speedPenalty > 0)
                {
                    tooltipSpeedPenalty.Q<Label>().text = $"- {weapon.speedPenalty} {(isEnglish ? "Speed Loss" : "Perda de Velocidade")}";
                    tooltipSpeedPenalty.style.display = DisplayStyle.Flex;
                }

                if (weapon.fireAttack > 0)
                {
                    tooltipFire.Q<Label>().text = $"+ {weapon.fireAttack} {(isEnglish ? "Fire Attack" : "Ataque de Fogo")}";
                    tooltipFire.style.display = DisplayStyle.Flex;
                }

                if (weapon.frostAttack > 0)
                {
                    tooltipFrost.Q<Label>().text = $"+ {weapon.frostAttack} {(isEnglish ? "Frost Attack" : "Ataque de Gelo")}";
                    tooltipFrost.style.display = DisplayStyle.Flex;
                }

                if (weapon.lightningAttack > 0)
                {
                    tooltipLightning.Q<Label>().text = $"+ {weapon.lightningAttack} {(isEnglish ? "Lightning Attack" : "Ataque de Relâmpago")}";
                    tooltipLightning.style.display = DisplayStyle.Flex;
                }

                if (weapon.magicAttack > 0)
                {
                    tooltipMagic.Q<Label>().text = $"+ {weapon.magicAttack} {(isEnglish ? "Magic Attack" : "Ataque Mágico")}";
                    tooltipMagic.style.display = DisplayStyle.Flex;
                }

                if (weapon.weaponAttackType == WeaponAttackType.Blunt)
                {
                    tooltipWeaponType.Q<Label>().text = $"{(isEnglish ? "Damage Type: Blunt" : "Tipo de Dano: Contusão")}";
                    tooltipWeaponType.style.display = DisplayStyle.Flex;
                }
                if (weapon.weaponAttackType == WeaponAttackType.Pierce)
                {
                    tooltipWeaponType.Q<Label>().text = $"{(isEnglish ? "Damage Type: Pierce" : "Tipo de Dano: Perfuração")}";
                    tooltipWeaponType.style.display = DisplayStyle.Flex;
                }
                if (weapon.weaponAttackType == WeaponAttackType.Slash)
                {
                    tooltipWeaponType.Q<Label>().text = $"{(isEnglish ? "Damage Type: Slash" : "Tipo de Dano: Corte")}";
                    tooltipWeaponType.style.display = DisplayStyle.Flex;
                }

                if (weapon.weaponSpecial != null)
                {
                    tooltipWeaponSpecial.Q<Label>().text = weapon.weaponSpecialDescription.GetText();
                    tooltipWeaponSpecial.style.display = DisplayStyle.Flex;
                }


                string strengthScaling = isEnglish
                    ? $"+{strengthAttackBonus} Attack [{weapon.strengthScaling}] (Strength Scaling)"
                    : $"+{strengthAttackBonus} Ataque [{weapon.strengthScaling}] (Bónus de Força)";
                tooltipWeaponStrengthScaling.Q<Label>().text = strengthScaling;
                tooltipWeaponStrengthScaling.style.display = DisplayStyle.Flex;

                string dexterityScaling = isEnglish
                    ? $"+{dexterityAttackBonus} Attack [{weapon.dexterityScaling}] (Dexterity Scaling)"
                    : $"+{dexterityAttackBonus} Ataque [{weapon.dexterityScaling}] (Bónus de Destreza)";
                tooltipWeaponDexterityScaling.Q<Label>().text = dexterityScaling;
                tooltipWeaponDexterityScaling.style.display = DisplayStyle.Flex;

                if (weapon.statusEffects != null && weapon.statusEffects.Length > 0)
                {
                    foreach (var statusEffect in weapon.statusEffects)
                    {
                        if (statusEffect.statusEffect.name.GetEnglishText() == "Bleed")
                        {
                            tooltipBleed.Q<Label>().text = $"{(isEnglish ? $"Bleed per hit: {statusEffect.amountPerHit}" : $"Dano de sangramento: {statusEffect.amountPerHit}")}";
                            tooltipBleed.style.display = DisplayStyle.Flex;
                        }

                        if (statusEffect.statusEffect.name.GetEnglishText() == "Poison")
                        {
                            tooltipPoison.Q<Label>().text = $"{(isEnglish ? $"Poison per hit: {statusEffect.amountPerHit}" : $"Dano de veneno: {statusEffect.amountPerHit}")}";
                            tooltipPoison.style.display = DisplayStyle.Flex;
                        }
                    }
                }


                tooltipBlockAbsorption.Q<Label>().text = $"{(isEnglish ? $"Block Absorption: {weapon.blockAbsorption}%" : $"Bloqueio de Dano: {weapon.blockAbsorption}%")}";
                tooltipBlockAbsorption.style.display = DisplayStyle.Flex;
            }
            #endregion

            if (item is Shield shield) {
                tooltipBlockAbsorption.Q<Label>().text = $"% {shield.defenseAbsorption} {(isEnglish ? "Damage Absorption" : "Absorção de Danos")}";
                tooltipBlockAbsorption.style.display = DisplayStyle.Flex;
            }

            #region Armor Logic
            if (item is ArmorBase armor)
            {

                tooltipPhysicalDefense.Q<Label>().text = $"+ {armor.physicalDefense} {(isEnglish ? "Physical Defense" : "Defesa Física")}";
                tooltipPhysicalDefense.style.display = armor.physicalDefense > 0 ? DisplayStyle.Flex : DisplayStyle.None;

                if (armor.speedPenalty > 0)
                {
                    tooltipSpeedPenalty.Q<Label>().text = $"- {armor.speedPenalty} {(isEnglish ? "Speed Loss" : "Perda de Velocidade")}";
                    tooltipSpeedPenalty.style.display = DisplayStyle.Flex;
                }

                if (armor.fireDefense > 0)
                {
                    tooltipFire.Q<Label>().text = $"+ {armor.fireDefense} {(isEnglish ? "Fire Defense" : "Defesa de Fogo")}";
                    tooltipFire.style.display = DisplayStyle.Flex;
                }
                if (armor.frostDefense > 0)
                {
                    tooltipFrost.Q<Label>().text = $"+ {armor.frostDefense} {(isEnglish ? "Frost Defense" : "Defesa de Gelo")}";
                    tooltipFrost.style.display = DisplayStyle.Flex;
                }
                if (armor.lightningDefense > 0)
                {
                    tooltipLightning.Q<Label>().text = $"+ {armor.lightningDefense} {(isEnglish ? "Lightning Defense" : "Defesa de Relâmpago")}";
                    tooltipLightning.style.display = DisplayStyle.Flex;
                }
                if (armor.magicDefense > 0)
                {
                    tooltipMagic.Q<Label>().text = $"+ {armor.magicDefense} {(isEnglish ? "Magic Defense" : "Defesa de Magia")}";
                    tooltipMagic.style.display = DisplayStyle.Flex;
                }

                if (armor.poiseBonus > 0)
                {
                    tooltipPoise.Q<Label>().text = $"+ {armor.poiseBonus} {(isEnglish ? "Poise" : "Postura")}";
                    tooltipPoise.style.display = DisplayStyle.Flex;
                }

                if (armor.statusEffectResistances != null && armor.statusEffectResistances.Length > 0)
                {
                    foreach (var statusEffect in armor.statusEffectResistances)
                    {
                        if (statusEffect.statusEffect.name.GetEnglishText() == "Bleed")
                        {
                            tooltipBleed.Q<Label>().text = $"{(isEnglish ? $"Bleed Resistance: {statusEffect.resistanceBonus}" : $"Resistência a sangramento: {statusEffect.resistanceBonus}")}";
                            tooltipBleed.style.display = DisplayStyle.Flex;
                        }

                        if (statusEffect.statusEffect.name.GetEnglishText() == "Poison")
                        {
                            tooltipPoison.Q<Label>().text = $"{(isEnglish ? $"Poison Resistance: {statusEffect.resistanceBonus}" : $"Resistência a veneno: {statusEffect.resistanceBonus}")}";
                            tooltipPoison.style.display = DisplayStyle.Flex;
                        }
                    }
                }
                 
                if (armor.additionalCoinPercentage != 0)
                {
                    tooltipGold.Q<Label>().text = $"+ %{armor.additionalCoinPercentage} {(isEnglish ? "gold found on enemies" : "ouro encontrado em inimigos")}";
                    tooltipGold.style.display = DisplayStyle.Flex;
                }

                if (armor.vitalityBonus != 0)
                {
                    tooltipVitality.Q<Label>().text = $"+ {armor.vitalityBonus} {(isEnglish ? "Vitality" : "Vitalidade")}";
                    tooltipVitality.style.display = DisplayStyle.Flex;
                }
                if (armor.enduranceBonus != 0)
                {
                    tooltipEndurance.Q<Label>().text = $"+ {armor.enduranceBonus} {(isEnglish ? "Endurance" : "Resistência")}";
                    tooltipEndurance.style.display = DisplayStyle.Flex;
                }
                if (armor.strengthBonus != 0)
                {
                    tooltipStrength.Q<Label>().text = $"+ {armor.strengthBonus} {(isEnglish ? "Strength" : "Força")}";
                    tooltipStrength.style.display = DisplayStyle.Flex;
                }
                if (armor.dexterityBonus != 0)
                {
                    tooltipDexterity.Q<Label>().text = $"+ {armor.dexterityBonus} {(isEnglish ? "Dexterity" : "Destreza")}";
                    tooltipDexterity.style.display = DisplayStyle.Flex;
                }
                if (armor.intelligenceBonus != 0)
                {
                    tooltipIntelligence.Q<Label>().text = $"+ {armor.intelligenceBonus} {(isEnglish ? "Intelligence" : "Inteligência")}";
                    tooltipIntelligence.style.display = DisplayStyle.Flex;
                }
                if (armor.reputationBonus != 0)
                {
                    tooltipReputationBonus.Q<Label>().text = (armor.reputationBonus > 0 ? "+" : "-") + armor.reputationBonus + " " + (isEnglish ? "Reputation" : "Reputação");
                    tooltipReputationBonus.style.display = DisplayStyle.Flex;
                }

            }
            #endregion

            if (item is Accessory accessory)
            {
                tooltipAccessoryProperty.Q<Label>().text = accessory.shortDescription.GetText();
                tooltipAccessoryProperty.style.display = DisplayStyle.Flex;
            }

            if (item is Consumable consumable)
            {
                tooltipConsumableEffect.Q<Label>().text = consumable.shortDescription.GetText();
                tooltipConsumableEffect.style.display = DisplayStyle.Flex;
            }

        }

        void DisplayTooltip(Button parentButton)
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

        void HideTooltip()
        {
            tooltip.style.display = DisplayStyle.None;
        }
        #endregion

        #region Stat Changes Logic

        int GetElementalDefenseFromItem(ArmorBase armorBase, WeaponElementType weaponElementType)
        {
            int baseElementalDefense = GetBaseElementalDefense(weaponElementType);

            Dictionary<string, Dictionary<WeaponElementType, int>> equippedItemsDictionary = new ()
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

        int GetPoiseFromItem(ArmorBase armorBase) {
            var currentPoise = playerPoiseController.GetMaxPoise();
            if (armorBase is Helmet helmet) {
                var poiseFromEquippedItem = Player.instance.equippedHelmet != null ? Player.instance.equippedHelmet.poiseBonus : 0;
                currentPoise -= poiseFromEquippedItem;
                if (currentPoise < 0) currentPoise = 0;
                
                return currentPoise + helmet.poiseBonus;
            }
            
            if (armorBase is Armor armor) {
                var poiseFromEquippedItem = Player.instance.equippedArmor != null ? Player.instance.equippedArmor.poiseBonus : 0;
                currentPoise -= poiseFromEquippedItem;
                if (currentPoise < 0) currentPoise = 0;
                
                return currentPoise + armor.poiseBonus;
            }
            
            if (armorBase is Gauntlet gauntlet) {
                var poiseFromEquippedItem = Player.instance.equippedGauntlets != null ? Player.instance.equippedGauntlets.poiseBonus : 0;
                currentPoise -= poiseFromEquippedItem;
                if (currentPoise < 0) currentPoise = 0;
                
                return currentPoise + gauntlet.poiseBonus;
            }
            
            if (armorBase is Legwear legwear) {
                var poiseFromEquippedItem = Player.instance.equippedLegwear != null ? Player.instance.equippedLegwear.poiseBonus : 0;
                currentPoise -= poiseFromEquippedItem;
                if (currentPoise < 0) currentPoise = 0;
                
                return currentPoise + legwear.poiseBonus;
            }
            
            if (armorBase is Accessory accessory) {
                var poiseBonus = 0;
                if (Player.instance.IsAccessoryEquiped(accessory) == false)
                {
                    poiseBonus = accessory.poiseBonus;
                }

                return currentPoise + poiseBonus;
            }

            return 0;
        }

        enum AttributeType { VITALITY, ENDURANCE, DEXTERITY, STRENGTH, INTELLIGENCE, REPUTATION };
        int GetAttributeFromEquipment(ArmorBase armorBase, AttributeType attributeType)
        {
            int currentValue = 0;
            int bonusFromEquipment = 0;

            if (attributeType == AttributeType.VITALITY) {
                currentValue = Player.instance.vitality + equipmentGraphicsHandler.vitalityBonus;
                bonusFromEquipment = armorBase.vitalityBonus;
            }
            else if (attributeType == AttributeType.ENDURANCE) {
                currentValue = Player.instance.endurance + equipmentGraphicsHandler.enduranceBonus;
                bonusFromEquipment = armorBase.enduranceBonus;
            }
            else if (attributeType == AttributeType.STRENGTH) {
                currentValue = Player.instance.strength + equipmentGraphicsHandler.strengthBonus;
                bonusFromEquipment = armorBase.strengthBonus;
            }
            else if (attributeType == AttributeType.DEXTERITY)
            {
                currentValue = Player.instance.dexterity + equipmentGraphicsHandler.dexterityBonus;
                bonusFromEquipment = armorBase.dexterityBonus;
            }
            else if (attributeType == AttributeType.INTELLIGENCE)
            {
                currentValue = Player.instance.intelligence + equipmentGraphicsHandler.intelligenceBonus;
                bonusFromEquipment = armorBase.intelligenceBonus;
            }
            else if (attributeType == AttributeType.REPUTATION) {
                currentValue = Player.instance.GetCurrentReputation();
                bonusFromEquipment = armorBase.reputationBonus;
            }

            int valueFromCurrentEquipment = 0;
            if (armorBase is Helmet)
            {
                if (Player.instance.equippedHelmet != null) {

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

                /*if (Player.instance.equippedAccessory != null)
                {

                    if (attributeType == AttributeType.VITALITY) { valueFromCurrentEquipment = Player.instance.equippedAccessory.vitalityBonus; }
                    else if (attributeType == AttributeType.ENDURANCE) { valueFromCurrentEquipment = Player.instance.equippedAccessory.enduranceBonus; }
                    else if (attributeType == AttributeType.STRENGTH) { valueFromCurrentEquipment = Player.instance.equippedAccessory.strengthBonus; }
                    else if (attributeType == AttributeType.DEXTERITY) { valueFromCurrentEquipment = Player.instance.equippedAccessory.dexterityBonus; }
                    else if (attributeType == AttributeType.REPUTATION) { valueFromCurrentEquipment = Player.instance.equippedAccessory.reputationBonus; }
                }*/
            }

            currentValue -= valueFromCurrentEquipment;
            if (currentValue < 0) currentValue = 0;

            return currentValue + bonusFromEquipment;
        }

        void UpdateStatLabel(Label label, string labelPrefix, int leftValue, int rightValue) {
            label.text = labelPrefix;

            if (rightValue != 0) {
                label.text += rightValue;

                if (rightValue > leftValue) {
                    label.style.color = Color.green;
                }
                else if (rightValue < leftValue) {
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
            if (rightValue == -1) {

                if (leftValue > 0)
                {
                    label.text += leftValue;
                    label.style.display = DisplayStyle.Flex;
                }

                return;
            }

            // New equipment has no elemental property
            if (rightValue == 0) {
                label.style.display = DisplayStyle.None;
                return;
            }

            // New equipment has elemental property
            label.text += rightValue;
            label.style.display = DisplayStyle.Flex;
        }

        public void DrawStats(Item item)
        {
            bool isEnglish = GamePreferences.instance.IsEnglish();


            statsAndAttributesLabels[LEVEL].text = (isEnglish ? "Level " : "Nível ") + playerLevelManager.GetCurrentLevel();
            statsAndAttributesLabels[GOLD].text =  (isEnglish ? "Gold " : "Ouro ") + Player.instance.currentGold;
            
            #region Attack
            int baseAttack = Player.instance.equippedWeapon != null
                ? attackStatManager.GetWeaponAttack(Player.instance.equippedWeapon)
                : attackStatManager.GetCurrentPhysicalAttack();

            int itemAttack = 0;
            if (item is Weapon weapon) {
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
            #endregion

            #region Stats and Physical Defense
            int baseVitality = Player.instance.vitality + equipmentGraphicsHandler.vitalityBonus;
            int baseEndurance = Player.instance.endurance + equipmentGraphicsHandler.enduranceBonus;
            int baseStrength = Player.instance.strength + equipmentGraphicsHandler.strengthBonus;
            int baseDexterity = Player.instance.dexterity + equipmentGraphicsHandler.dexterityBonus;
            int baseIntelligence = Player.instance.intelligence + equipmentGraphicsHandler.intelligenceBonus;
            int baseReputation = Player.instance.GetCurrentReputation();
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

            if (item is ArmorBase armorBase) {

                // EDGE CASDE: If is not accessory that is already equipped
                if ((armorBase is Accessory acc && Player.instance.IsAccessoryEquiped(acc)) == false)
                {
                    itemPhysicalDefense = GetElementalDefenseFromItem(armorBase, WeaponElementType.None);
                    itemFireDefense = GetElementalDefenseFromItem(armorBase, WeaponElementType.Fire);
                    itemFrostDefense = GetElementalDefenseFromItem(armorBase, WeaponElementType.Frost);
                    itemLightningDefense = GetElementalDefenseFromItem(armorBase, WeaponElementType.Lightning);
                    itemMagicDefense = GetElementalDefenseFromItem(armorBase, WeaponElementType.Magic);
                }
                itemPoise = GetPoiseFromItem(armorBase);
                vitalityFromItem = GetAttributeFromEquipment(armorBase, AttributeType.VITALITY);
                enduranceFromItem = GetAttributeFromEquipment(armorBase, AttributeType.ENDURANCE);
                strengthFromItem = GetAttributeFromEquipment(armorBase, AttributeType.STRENGTH);
                dexterityFromItem = GetAttributeFromEquipment(armorBase, AttributeType.DEXTERITY);
                reputationFromItem = GetAttributeFromEquipment(armorBase, AttributeType.REPUTATION);
                intelligenceFromItem = GetAttributeFromEquipment(armorBase, AttributeType.INTELLIGENCE);
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
            #endregion
        }
        #endregion

        #region Equipment Better or Worse Indicator
        public Color GetEquipmentColorIndicator<T>(T item) where T : Item
        {
            bool shouldReturn = false;
            int value = 0;
            if (item is Weapon weapon)
            {
                value = attackStatManager.CompareWeapon(weapon);
                shouldReturn = true;
            }
            else if (item is Helmet helmet)
            {
                value = defenseStatManager.CompareHelmet(helmet);
                shouldReturn = true;
            }
            else if (item is Armor armor)
            {
                value = defenseStatManager.CompareArmor(armor);
                shouldReturn = true;
            }
            else if (item is Gauntlet gauntlet)
            {
                value = defenseStatManager.CompareGauntlet(gauntlet);
                shouldReturn = true;
            }
            else if (item is Legwear legwear)
            {
                value = defenseStatManager.CompareLegwear(legwear);
                shouldReturn = true;
            }

            if (shouldReturn)
            {
                if (value > 0) return Color.green;
                else if (value == 0) return Color.yellow;
                else if (value < 0) return Color.red;
            }

            return Color.black;
        }
        #endregion

    }

}
