using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace AF
{
    public class UIDocumentMainMenu : UIDocumentBase
    {
        [Header("Main Menu Buttons")]
        public Button equipmentButton;
        public Button inventoryButton;
        public Button managePartyButton;
        public Button saveGameButton;
        public Button loadGameButton;
        public Button exitGameButton;

        [Header("Character Stats Labels")]
        public Label level;
        public Label currentExperience;
        public Label hp;
        public Label mp;
        public Label stamina;
        public Label reputation;
        public Label vitality;
        public Label endurance;
        public Label intelligence;
        public Label strength;
        public Label dexterity;
        public Label arcane;
        public Label occult;
        public Label charisma;
        public Label attackPower;
        public Label physicalAttack;
        public Label weaponAttack;
        public Label scalingBonus;
        public Label defenseAbsorption;
        public Label physicalDefense;
        public Label equipmentDefense;

        MenuManager menuManager;

        private void Awake()
        {
            menuManager = FindObjectOfType<MenuManager>(true);
        }

        protected override void Start()
        {
            base.Start();

            this.equipmentButton = root.Q<Button>("EquipmentButton");
            SetupButtonClick(this.equipmentButton, () =>
            {
                UIDocumentEquipmentMenu equipmentMenu = FindObjectOfType<UIDocumentEquipmentMenu>(true);

                if (equipmentMenu != null)
                {
                    equipmentMenu.Enable();

                    this.Disable();
                }
            });

            this.inventoryButton = root.Q<Button>("InventoryButton");
            SetupButtonClick(this.inventoryButton, () => {
                UIDocumentInventoryMenu inventoryMenu = FindObjectOfType<UIDocumentInventoryMenu>(true);

                if (inventoryMenu != null)
                {
                    inventoryMenu.Enable();

                    this.Disable();
                }
            });

            this.managePartyButton = root.Q<Button>("ManagePartyButton");
            SetupButtonClick(this.managePartyButton, () => { });

            this.saveGameButton = root.Q<Button>("SaveGameButton");
            SetupButtonClick(this.saveGameButton, () => {
                SaveSystem.instance.SaveGameData();
                this.Disable();

                FindObjectOfType<UIDocumentPlayerHUD>(true).Enable();
            });

            this.loadGameButton = root.Q<Button>("LoadGameButton");
            SetupButtonClick(this.loadGameButton, () => {
                SaveSystem.instance.LoadGameData();
                this.Disable();
            });

            this.exitGameButton = root.Q<Button>("ExitGameButton");
            SetupButtonClick(this.exitGameButton, () => { });

            this.level = root.Q<VisualElement>("Level").Q<Label>("Value");
            this.currentExperience = root.Q<VisualElement>("CurrentExperience").Q<Label>("Value");
            this.hp = root.Q<VisualElement>("CurrentHP").Q<Label>("Value");
            this.mp = root.Q<VisualElement>("CurrentMP").Q<Label>("Value");
            this.stamina = root.Q<VisualElement>("CurrentStamina").Q<Label>("Value");
            this.reputation = root.Q<VisualElement>("Reputation").Q<Label>("Value");
            this.vitality = root.Q<VisualElement>("Vitality").Q<Label>("Value");
            this.endurance = root.Q<VisualElement>("Endurance").Q<Label>("Value");
            this.intelligence = root.Q<VisualElement>("Intelligence").Q<Label>("Value");
            this.strength = root.Q<VisualElement>("Strength").Q<Label>("Value");
            this.dexterity = root.Q<VisualElement>("Dexterity").Q<Label>("Value");
            this.arcane = root.Q<VisualElement>("Arcane").Q<Label>("Value");
            this.charisma = root.Q<VisualElement>("Charisma").Q<Label>("Value");
            this.attackPower = root.Q<VisualElement>("AttackPower").Q<Label>("Value");
            this.physicalAttack = root.Q<VisualElement>("PhysicalAttack").Q<Label>("Value");
            this.weaponAttack = root.Q<VisualElement>("WeaponBaseAttack").Q<Label>("Value");
            this.scalingBonus = root.Q<VisualElement>("WeaponScalingBonus").Q<Label>("Value");
            this.scalingBonus.text = "+0 (STR: C)\n+0 (DEX: C)";

            this.defenseAbsorption = root.Q<VisualElement>("DefenseAbsorption").Q<Label>("Value");
            this.physicalDefense = root.Q<VisualElement>("PhysicalDefense").Q<Label>("Value");
            this.equipmentDefense = root.Q<VisualElement>("EquipmentDefense").Q<Label>("Value");

            base.Disable();
        }

        private void Update()
        {
            this.level.text = PlayerStatsManager.instance.GetCurrentLevel().ToString();
            this.currentExperience.text = PlayerStatsManager.instance.currentExperience + "/" + PlayerStatsManager.instance.GetRequiredExperienceForNextLevel();
            this.hp.text = PlayerStatsManager.instance.GetCurrentHealth() + "/" + PlayerStatsManager.instance.GetMaxHealthPoints();
            this.mp.text = PlayerStatsManager.instance.currentMagic + "/" + PlayerStatsManager.instance.GetMaxMagicPoints();
            this.stamina.text = PlayerStatsManager.instance.GetCurrentStamina() + "/" + PlayerStatsManager.instance.GetMaxStaminaPoints();
            this.currentExperience.text = PlayerStatsManager.instance.currentExperience + "/" + PlayerStatsManager.instance.GetRequiredExperienceForNextLevel();
            this.reputation.text = PlayerStatsManager.instance.currentReputation.ToString();
            this.vitality.text = PlayerStatsManager.instance.vitality.ToString();
            this.intelligence.text = PlayerStatsManager.instance.intelligence.ToString();
            this.endurance.text = PlayerStatsManager.instance.endurance.ToString();
            this.strength.text = PlayerStatsManager.instance.strength.ToString();
            this.dexterity.text = PlayerStatsManager.instance.dexterity.ToString();
            this.arcane.text = PlayerStatsManager.instance.arcane.ToString();
            this.charisma.text = PlayerStatsManager.instance.charisma.ToString();

            var weapon = PlayerInventoryManager.instance.currentWeapon;
            this.attackPower.text = PlayerStatsManager.instance.GetWeaponAttack(weapon).ToString();
            this.physicalAttack.text = " " + PlayerStatsManager.instance.GetLevelPhysicalAttack().ToString();
            this.weaponAttack.text = "+" + weapon.physicalAttack.ToString() + " (" + weapon.name + ")";

            var strengthBonus = PlayerStatsManager.instance.GetFinalStrengthBonusDifference(weapon);
            var strengthScaling = weapon.strengthScaling;
            var dexBonus = PlayerStatsManager.instance.GetFinalDexterityBonusDifference(weapon);
            var dexScaling = weapon.dexterityScaling;
            this.scalingBonus.text = "+"+strengthBonus+" (STR: "+strengthScaling+") / +"+dexBonus+" (DEX: "+dexScaling+")";

            this.defenseAbsorption.text = PlayerStatsManager.instance.GetDefenseAbsorption().ToString();
            this.physicalDefense.text = " " + PlayerStatsManager.instance.GetLevelPhysicalDefense().ToString();
            this.equipmentDefense.text = "+" + PlayerStatsManager.instance.GetEquipmentDefenseAbsorption().ToString();

        }

        public override void Enable()
        {
            base.Enable();

            this.equipmentButton.Focus();
        }

        public override void Disable()
        {
            base.Disable();

            menuManager.PlayCancelSfx();
        }
    }
}
