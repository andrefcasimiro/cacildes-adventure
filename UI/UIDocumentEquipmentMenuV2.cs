using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentEquipmentMenuV2 : MonoBehaviour
    {
        MenuManager menuManager;

        AttackStatManager attackStatManager;
        DefenseStatManager defenseStatManager;

        [Header("Settings")]
        public Sprite emptyWeaponSprite;
        public Sprite emptyShieldSprite;
        public Sprite emptyHeadSprite;
        public Sprite emptyChestSprite;
        public Sprite emptyGauntletsSprite;
        public Sprite emptyLegwearSprite;
        public Sprite emptyAccessorySprite;

        VisualElement root;

        private void Awake()
        {
            attackStatManager = FindObjectOfType<AttackStatManager>(true);
            defenseStatManager = FindObjectOfType<DefenseStatManager>(true);
            menuManager = FindObjectOfType<MenuManager>(true);
        }

        private void OnEnable()
        {
            this.root = GetComponent<UIDocument>().rootVisualElement;

            menuManager.SetupNavMenu(this.root);
            menuManager.TranslateNavbar(root);

            menuManager.SetActiveMenu(this.root, "ButtonEquipment");

            var player = Player.instance;

            var playerEquippedWeaponAttack = player.equippedWeapon != null ? attackStatManager.GetWeaponAttack(player.equippedWeapon) : attackStatManager.GetCurrentPhysicalAttack();

            var weaponBonus = Mathf.Abs(playerEquippedWeaponAttack - attackStatManager.GetCurrentPhysicalAttack());
            var defenseBonus = Mathf.Abs(defenseStatManager.GetDefenseAbsorption() - defenseStatManager.GetCurrentPhysicalDefense());
            this.root.Q<VisualElement>("EquipmentStats").Q<VisualElement>("Attack").Q<Label>("Value").text = 
                "/ " + ((player.equippedWeapon != null ? attackStatManager.GetWeaponAttack(player.equippedWeapon) : attackStatManager.GetCurrentPhysicalAttack())) + "";
            this.root.Q<VisualElement>("EquipmentStats").Q<VisualElement>("Attack").Q<Label>("Explanation").text = 
                "" + attackStatManager.GetCurrentPhysicalAttack() + " (Base) + " + weaponBonus.ToString() + " (" + LocalizedTerms.Weapon() + ")";
            this.root.Q<VisualElement>("EquipmentStats").Q<VisualElement>("Defense").Q<Label>("Value").text = 
                "/ " + (defenseBonus + defenseStatManager.GetCurrentPhysicalDefense()).ToString() + "";
            this.root.Q<VisualElement>("EquipmentStats").Q<VisualElement>("Defense").Q<Label>("Explanation").text = 
                "" + defenseStatManager.GetCurrentPhysicalDefense() + " (Base) + " + defenseBonus.ToString() + " (" + LocalizedTerms.Gear() + ")";

            this.root.Q<Label>("Attributes").text =
                    LocalizedTerms.Vitality() + " " + player.vitality + " / " +
                    LocalizedTerms.Endurance() + " " + player.endurance + " / " +
                    LocalizedTerms.Strength() + " " + player.strength + " / " +
                    LocalizedTerms.Dexterity() + " " + player.dexterity;

            var healthStatManager = FindObjectOfType<HealthStatManager>(true);
            var staminaStatManager = FindObjectOfType<StaminaStatManager>(true);
            var playerPoiseController = FindObjectOfType<PlayerPoiseController>(true);
            this.root.Q<Label>("Stats").text =
                    "HP " + healthStatManager.GetMaxHealth()
                    + " / Stamina " + staminaStatManager.GetMaxStamina()
                    + " / " + LocalizedTerms.Poise() + " " + playerPoiseController.GetMaxPoise()
                    + " / " + LocalizedTerms.Reputation() + " " + player.currentReputation
                    + " / " + LocalizedTerms.Gold() + " " + player.currentGold;

            // Weapon
            var weaponButtonElement = this.root.Q<VisualElement>("Weapon").Q<VisualElement>("Root").Q<Button>();
            weaponButtonElement.Q<Label>("Label").text = player.equippedWeapon != null ? player.equippedWeapon.name.GetText() : LocalizedTerms.Weapon();
            weaponButtonElement.Q<Label>("Value").text = player.equippedWeapon != null
                ? "+ " + (attackStatManager.GetWeaponAttack(player.equippedWeapon) - attackStatManager.GetCurrentPhysicalAttack()) + " " + LocalizedTerms.ATK()
                : "+ 0 " + LocalizedTerms.ATK();

            if (player.equippedWeapon != null)
            {
                weaponButtonElement.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(player.equippedWeapon.sprite);
                weaponButtonElement.Q<IMGUIContainer>("Icon").style.opacity = 1f;
            }
            else
            {
                weaponButtonElement.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(emptyWeaponSprite);
            }
            menuManager.SetupButton(weaponButtonElement, () =>
            {
                menuManager.OpenEquipmentSelectionScreen(UIDocumentEquipmentSelectionMenuV2.EquipmentType.Weapon);
            });

            // Shield
            var shieldButtonElement = this.root.Q<VisualElement>("Shield").Q<VisualElement>("Root").Q<Button>();
            shieldButtonElement.Q<Label>("Label").text = player.equippedShield != null ? player.equippedShield.name.GetText() : LocalizedTerms.Shield();
            shieldButtonElement.Q<Label>("Value").text = player.equippedShield != null
                ? "" + player.equippedShield.defenseAbsorption + "% " + LocalizedTerms.DEFAbsorption()
                : "0% " + LocalizedTerms.DEFAbsorption();

            if (player.equippedShield != null)
            {
                shieldButtonElement.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(player.equippedShield.sprite);
                shieldButtonElement.Q<IMGUIContainer>("Icon").style.opacity = 1f;
            }
            else
            {
                shieldButtonElement.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(emptyShieldSprite);
            }
            menuManager.SetupButton(shieldButtonElement, () =>
            {
                menuManager.OpenEquipmentSelectionScreen(UIDocumentEquipmentSelectionMenuV2.EquipmentType.Shield);
            });

            CreateArmorEquipmentButton(this.root, "Head", LocalizedTerms.Helmet(), UIDocumentEquipmentSelectionMenuV2.EquipmentType.Helmet, Player.instance.equippedHelmet, emptyHeadSprite);
            CreateArmorEquipmentButton(this.root, "Chest", LocalizedTerms.Armor(), UIDocumentEquipmentSelectionMenuV2.EquipmentType.Armor, Player.instance.equippedArmor, emptyChestSprite);
            CreateArmorEquipmentButton(this.root, "Hands", LocalizedTerms.Gauntlets(), UIDocumentEquipmentSelectionMenuV2.EquipmentType.Gauntlets, Player.instance.equippedGauntlets, emptyGauntletsSprite);
            CreateArmorEquipmentButton(this.root, "Legs", LocalizedTerms.Boots(), UIDocumentEquipmentSelectionMenuV2.EquipmentType.Legwear, Player.instance.equippedLegwear, emptyLegwearSprite);

            // Accessory
            var accessoryElement = this.root.Q<VisualElement>("Accessory").Q<VisualElement>("Root").Q<Button>();
            accessoryElement.Q<Label>("Label").text = player.equippedAccessory != null ? player.equippedAccessory.name.GetText() : LocalizedTerms.Accessory();
            accessoryElement.Q<Label>("Value").text = player.equippedAccessory != null
                ? player.equippedAccessory.smallEffectDescription.GetText()
                : "";

            if (player.equippedAccessory != null)
            {
                accessoryElement.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(player.equippedAccessory.sprite);
                accessoryElement.Q<IMGUIContainer>("Icon").style.opacity = 1f;
            }
            else
            {
                accessoryElement.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(emptyAccessorySprite);
            }
            accessoryElement.RegisterCallback<ClickEvent>(ev =>
            {
                menuManager.OpenEquipmentSelectionScreen(UIDocumentEquipmentSelectionMenuV2.EquipmentType.Accessories);
            });

            // Favorite items
            var favoriteItemsElement = this.root.Q<VisualElement>("FavoriteItems").Q<VisualElement>("Root").Q<Button>();

            favoriteItemsElement.Q<Label>("FavoriteItemsLabel").text = LocalizedTerms.FavoriteItems() + " ";

            Sprite[] sprites = (Player.instance.favoriteItems.Select(x => x.sprite)).ToArray();


            foreach (var sprite in sprites)
            {
                var favoriteItemSprite = new IMGUIContainer();
                favoriteItemSprite.style.height = 25;
                favoriteItemSprite.style.width = 25;
                favoriteItemSprite.style.marginRight = 5;
                favoriteItemSprite.style.borderBottomWidth = 1;
                favoriteItemSprite.style.borderRightWidth = 1;
                favoriteItemSprite.style.borderTopWidth = 1;
                favoriteItemSprite.style.borderLeftWidth = 1;
                favoriteItemSprite.style.borderBottomColor = Color.white;
                favoriteItemSprite.style.borderLeftColor = Color.white;
                favoriteItemSprite.style.borderRightColor = Color.white;
                favoriteItemSprite.style.borderTopColor = Color.white;
                favoriteItemSprite.style.backgroundImage = new StyleBackground(sprite);
                favoriteItemsElement.Q<VisualElement>("IconsContainer").Add(favoriteItemSprite);
            }

            favoriteItemsElement.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(emptyAccessorySprite);
            if (player.favoriteItems.Count > 0)
            {
                accessoryElement.Q<IMGUIContainer>("Icon").style.opacity = 1f;
            }

            favoriteItemsElement.RegisterCallback<ClickEvent>(ev =>
            {
                menuManager.OpenEquipmentSelectionScreen(UIDocumentEquipmentSelectionMenuV2.EquipmentType.FavoriteItems);
            });

        }

        void CreateArmorEquipmentButton(
            VisualElement root,
            string name,
            string localizedName,
            UIDocumentEquipmentSelectionMenuV2.EquipmentType equipmentType,
            ArmorBase equippedItem,
            Sprite emptySprite)
        {
            var btnElement = root.Q<VisualElement>(name).Q<VisualElement>("Root").Q<Button>();

            btnElement.Q<Label>("Label").text = equippedItem != null ? equippedItem.name.GetText() : localizedName;
            btnElement.Q<Label>("Value").text = equippedItem != null
                ? "+ " + equippedItem.physicalDefense + " DEF"
                : "+ 0 DEF";

            if (equippedItem != null)
            {
                btnElement.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(equippedItem.sprite);
                btnElement.Q<IMGUIContainer>("Icon").style.opacity = 1f;
            }
            else
            {
                btnElement.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(emptySprite);
            }

            menuManager.SetupButton(btnElement, () =>
            {
                menuManager.OpenEquipmentSelectionScreen(equipmentType);
            });

        }
    }
}
