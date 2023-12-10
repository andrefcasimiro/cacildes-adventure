using AF.Events;
using AF.Inventory;
using AF.Stats;
using TigerForge;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace AF
{

    public class UIDocumentPlayerHUDV2 : MonoBehaviour
    {

        UIDocument uIDocument => GetComponent<UIDocument>();
        public VisualElement root;

        VisualElement healthContainer;
        VisualElement healthFill;
        VisualElement staminaContainer;
        VisualElement staminaFill;

        [Header("Graphic Settings")]
        public float healthContainerBaseWidth = 185;
        public float staminaContainerBaseWidth = 150;
        float _containerMultiplierPerLevel = 6.5f;

        Label quickItemName, arrowsLabel;
        IMGUIContainer shieldBlockedIcon;


        [Header("Components")]
        public StatsBonusController playerStatsBonusController;

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;
        public InventoryDatabase inventoryDatabase;
        public PlayerStatsDatabase playerStatsDatabase;
        public QuestsDatabase questsDatabase;

        [Header("Unequipped Textures")]
        public Texture2D unequippedSpellSlot;
        public Texture2D unequippedWeaponSlot;
        public Texture2D unequippedConsumableSlot;
        public Texture2D unequippedShieldSlot;
        public Texture2D unequippedArrowSlot;

        [Header("Components")]
        public PlayerHealth healthStatManager;
        public StaminaStatManager staminaStatManager;
        public EquipmentGraphicsHandler equipmentGraphicsHandler;

        IMGUIContainer spellSlotContainer, consumableSlotContainer, weaponSlotContainer, shieldSlotContainer;

        [Header("Animations")]
        public Vector3 popEffectWhenSwitchingSlots = new Vector3(0.8f, 0.8f, 0.8f);

        VisualElement leftGamepad, alpha1, upGamepad, alpha2, rightGamepad, alpha3, downGamepad, alpha4;
        VisualElement useKeyboard, useGamepad;
        VisualElement equipmentContainer;

        Label currentObjectiveLabel, currentObjectiveValue;

        private void Awake()
        {
            EventManager.StartListening(
                EventMessages.ON_EQUIPMENT_CHANGED,
                UpdateEquipment);

            EventManager.StartListening(
                EventMessages.ON_QUEST_TRACKED,
                UpdateQuestTracking);

            EventManager.StartListening(
                EventMessages.ON_QUEST_OBJECTIVE_COMPLETED,
                UpdateQuestTracking);
        }

        private void OnEnable()
        {
            this.root = this.uIDocument.rootVisualElement;
            healthContainer = root.Q<VisualElement>("Health");
            healthFill = root.Q<VisualElement>("HealthFill");
            staminaContainer = root.Q<VisualElement>("Stamina");
            staminaFill = root.Q<VisualElement>("StaminaFill");

            quickItemName = root.Q<Label>("QuickItemName");
            arrowsLabel = root.Q<Label>("ArrowsLabel");

            spellSlotContainer = root.Q<IMGUIContainer>("SpellSlot");
            consumableSlotContainer = root.Q<IMGUIContainer>("ConsumableSlot");
            weaponSlotContainer = root.Q<IMGUIContainer>("WeaponSlot");
            shieldSlotContainer = root.Q<IMGUIContainer>("ShieldSlot");

            shieldBlockedIcon = shieldSlotContainer.Q<IMGUIContainer>("Blocked");

            leftGamepad = shieldSlotContainer.Q<VisualElement>("Gamepad");
            alpha1 = shieldSlotContainer.Q<VisualElement>("Keyboard");

            upGamepad = spellSlotContainer.Q<VisualElement>("Gamepad");
            alpha2 = spellSlotContainer.Q<VisualElement>("Keyboard");

            rightGamepad = weaponSlotContainer.Q<VisualElement>("Gamepad");
            alpha3 = weaponSlotContainer.Q<VisualElement>("Keyboard");

            downGamepad = consumableSlotContainer.Q<VisualElement>("Gamepad");
            alpha4 = consumableSlotContainer.Q<VisualElement>("Keyboard");

            useKeyboard = consumableSlotContainer.Q<VisualElement>("UseKeyboard");
            useGamepad = consumableSlotContainer.Q<VisualElement>("UseGamepad");

            equipmentContainer = root.Q<VisualElement>("EquipmentContainer");

            currentObjectiveLabel = root.Q<Label>("CurrentObjectiveLabel");
            currentObjectiveValue = root.Q<Label>("CurrentObjectiveValue");
            currentObjectiveLabel.style.display = DisplayStyle.None;
            currentObjectiveValue.text = "";

            UpdateEquipment();
        }

        public void HideHUD()
        {
            root.visible = false;
        }
        public void ShowHUD()
        {
            root.visible = true;
        }

        private void Update()
        {
            healthContainer.style.width = healthContainerBaseWidth +
                ((playerStatsDatabase.vitality + playerStatsBonusController.vitalityBonus) * _containerMultiplierPerLevel);
            staminaContainer.style.width = staminaContainerBaseWidth + ((
                playerStatsDatabase.endurance + playerStatsBonusController.enduranceBonus) * _containerMultiplierPerLevel);

            float healthPercentage = healthStatManager.GetCurrentHealthPercentage();

            Length formattedHp = new Length(healthPercentage, LengthUnit.Percent);
            this.healthFill.style.width = formattedHp;


            float staminaPercentage = Mathf.Clamp(
                (playerStatsDatabase.currentStamina * 100) / staminaStatManager.GetMaxStamina(),
                0,
                100
            );

            Length formattedStamina = new Length(staminaPercentage, LengthUnit.Percent);
            this.staminaFill.style.width = formattedStamina;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ShowEquipment()
        {
            equipmentContainer.visible = true;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void HideEquipment()
        {
            equipmentContainer.visible = false;
        }

        public void UpdateEquipment()
        {
            if (!this.isActiveAndEnabled)
            {
                return;
            }

            quickItemName.text = "";
            arrowsLabel.text = "";


            alpha1.style.display = Gamepad.current == null ? DisplayStyle.Flex : DisplayStyle.None;
            alpha2.style.display = Gamepad.current == null ? DisplayStyle.Flex : DisplayStyle.None;
            alpha3.style.display = Gamepad.current == null ? DisplayStyle.Flex : DisplayStyle.None;
            alpha4.style.display = Gamepad.current == null ? DisplayStyle.Flex : DisplayStyle.None;

            leftGamepad.style.display = Gamepad.current != null ? DisplayStyle.Flex : DisplayStyle.None;
            upGamepad.style.display = Gamepad.current != null ? DisplayStyle.Flex : DisplayStyle.None;
            rightGamepad.style.display = Gamepad.current != null ? DisplayStyle.Flex : DisplayStyle.None;
            downGamepad.style.display = Gamepad.current != null ? DisplayStyle.Flex : DisplayStyle.None;

            useGamepad.style.display = equipmentDatabase.GetCurrentConsumable() != null && Gamepad.current != null ? DisplayStyle.Flex : DisplayStyle.None;
            useKeyboard.style.display = equipmentDatabase.GetCurrentConsumable() != null && Gamepad.current == null ? DisplayStyle.Flex : DisplayStyle.None;

            if (equipmentDatabase.IsBowEquipped())
            {
                arrowsLabel.text = equipmentDatabase.GetCurrentArrow() != null
                    ? equipmentDatabase.GetCurrentArrow().name.GetEnglishText() + " (" + inventoryDatabase.GetItemAmount(equipmentDatabase.GetCurrentArrow()) + ")"
                    : "";

                spellSlotContainer.style.backgroundImage = equipmentDatabase.GetCurrentArrow() != null
                    ? new StyleBackground(equipmentDatabase.GetCurrentArrow().sprite)
                    : new StyleBackground(unequippedArrowSlot);
            }
            else
            {
                spellSlotContainer.style.backgroundImage = equipmentDatabase.GetCurrentSpell() != null
                    ? new StyleBackground(equipmentDatabase.GetCurrentSpell().sprite)
                    : new StyleBackground(unequippedSpellSlot);
            }

            shieldSlotContainer.style.backgroundImage = equipmentDatabase.GetCurrentShield() != null
                ? new StyleBackground(equipmentDatabase.GetCurrentShield().sprite)
                : new StyleBackground(unequippedShieldSlot);

            shieldBlockedIcon.style.display = equipmentDatabase.IsBowEquipped() || equipmentDatabase.IsStaffEquipped()
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            weaponSlotContainer.style.backgroundImage = equipmentDatabase.GetCurrentWeapon() != null
                ? new StyleBackground(equipmentDatabase.GetCurrentWeapon().sprite)
                : new StyleBackground(unequippedWeaponSlot);

            quickItemName.text = equipmentDatabase.GetCurrentConsumable() != null ?
                equipmentDatabase.GetCurrentConsumable().name.GetEnglishText() + $" ({inventoryDatabase.GetItemAmount(equipmentDatabase.GetCurrentConsumable())})"
                : "";

            consumableSlotContainer.style.backgroundImage = equipmentDatabase.GetCurrentConsumable() != null
                ? new StyleBackground(equipmentDatabase.GetCurrentConsumable().sprite)
                : new StyleBackground(unequippedConsumableSlot);
        }

        public void OnSwitchWeapon()
        {
            UIUtils.PlayPopAnimation(weaponSlotContainer, popEffectWhenSwitchingSlots);
            UpdateEquipment();
        }
        public void OnSwitchShield()
        {
            UIUtils.PlayPopAnimation(shieldSlotContainer, popEffectWhenSwitchingSlots);
            UpdateEquipment();
        }
        public void OnSwitchConsumable()
        {
            UIUtils.PlayPopAnimation(consumableSlotContainer, popEffectWhenSwitchingSlots);
            UpdateEquipment();
        }
        public void OnSwitchSpell()
        {
            UIUtils.PlayPopAnimation(spellSlotContainer, popEffectWhenSwitchingSlots);
            UpdateEquipment();
        }

        public bool IsEquipmentDisplayed()
        {
            if (!root.visible)
            {
                return false;
            }

            return equipmentContainer.visible;
        }


        void UpdateQuestTracking()
        {
            currentObjectiveLabel.style.display = DisplayStyle.None;
            currentObjectiveValue.text = "";

            QuestObjective currentQuestObjective = questsDatabase.GetCurrentTrackedQuestObjective();

            if (currentQuestObjective != null)
            {
                currentObjectiveValue.text = currentQuestObjective.objective;
                currentObjectiveLabel.style.display = DisplayStyle.Flex;
            }
        }
    }
}
