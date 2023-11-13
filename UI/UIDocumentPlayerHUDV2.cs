using AF.Events;
using AF.Stats;
using TigerForge;
using UnityEngine;
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

        Label quickItemName;

        [Header("Components")]
        public StatsBonusController playerStatsBonusController;

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;
        public PlayerStatsDatabase playerStatsDatabase;

        [Header("Unequipped Textures")]
        public Texture2D unequippedSpellSlot;
        public Texture2D unequippedWeaponSlot;
        public Texture2D unequippedConsumableSlot;
        public Texture2D unequippedShieldSlot;

        [Header("Components")]
        public HealthStatManager healthStatManager;
        public StaminaStatManager staminaStatManager;
        public EquipmentGraphicsHandler equipmentGraphicsHandler;

        IMGUIContainer spellSlotContainer, consumableSlotContainer, weaponSlotContainer, shieldSlotContainer;


        private void Awake()
        {
            EventManager.StartListening(
                EventMessages.ON_EQUIPMENT_CHANGED,
                UpdateFavoriteItems);
        }

        private void OnEnable()
        {
            this.root = this.uIDocument.rootVisualElement;
            healthContainer = root.Q<VisualElement>("Health");
            healthFill = root.Q<VisualElement>("HealthFill");
            staminaContainer = root.Q<VisualElement>("Stamina");
            staminaFill = root.Q<VisualElement>("StaminaFill");

            quickItemName = root.Q<Label>("QuickItemName");

            spellSlotContainer = root.Q<IMGUIContainer>("SpellSlot");
            consumableSlotContainer = root.Q<IMGUIContainer>("ConsumableSlot");
            weaponSlotContainer = root.Q<IMGUIContainer>("WeaponSlot");
            shieldSlotContainer = root.Q<IMGUIContainer>("ShieldSlot");

            UpdateFavoriteItems();
        }

        public void HideHUD()
        {
            root.style.opacity = 0;
        }
        public void ShowHUD()
        {
            root.style.opacity = 1;
        }

        private void Update()
        {
            healthContainer.style.width = healthContainerBaseWidth +
                ((playerStatsDatabase.vitality + playerStatsBonusController.vitalityBonus) * _containerMultiplierPerLevel);
            staminaContainer.style.width = staminaContainerBaseWidth + ((
                playerStatsDatabase.endurance + playerStatsBonusController.enduranceBonus) * _containerMultiplierPerLevel);

            float healthPercentage = Mathf.Clamp(
                (Player.instance.currentHealth * 100) / healthStatManager.GetMaxHealth(),
                0,
                100
            );

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

        public void UpdateFavoriteItems()
        {
            if (!this.isActiveAndEnabled)
            {
                return;
            }

            quickItemName.text = "";

            quickItemName.text = equipmentDatabase.GetCurrentConsumable() != null ?
                equipmentDatabase.GetCurrentConsumable().name.GetEnglishText()
                : "";

            if (equipmentDatabase.GetCurrentConsumable() != null)
            {
                /*if (equipmentDatabase.currentConsumable is Consumable c && c.notStackable == false)
                {
                    quickItemName.text += " (" + equipmentDatabase.amount.ToString() + ")";
                }
                else if (favItem.item is Spell s)
                {
                    var spellName = favItem.item.name.GetEnglishText();

                    quickItemName.text += " (" + (favItem.amount).ToString() + ")";
                }*/
            }

            spellSlotContainer.style.backgroundImage = equipmentDatabase.GetCurrentSpell() != null
                ? new StyleBackground(equipmentDatabase.GetCurrentSpell().sprite)
                : new StyleBackground(unequippedSpellSlot);

            shieldSlotContainer.style.backgroundImage = equipmentDatabase.GetCurrentShield() != null
                ? new StyleBackground(equipmentDatabase.GetCurrentShield().sprite)
                : new StyleBackground(unequippedShieldSlot);

            weaponSlotContainer.style.backgroundImage = equipmentDatabase.GetCurrentWeapon() != null
                ? new StyleBackground(equipmentDatabase.GetCurrentWeapon().sprite)
                : new StyleBackground(unequippedWeaponSlot);

            consumableSlotContainer.style.backgroundImage = equipmentDatabase.GetCurrentConsumable() != null
                ? new StyleBackground(equipmentDatabase.GetCurrentConsumable().sprite)
                : new StyleBackground(unequippedConsumableSlot);

        }

    }

}
