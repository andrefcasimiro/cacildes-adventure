using System.Collections;
using System.Collections.Generic;
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
        public float containerMultiplierPerLevel = 1.5f;

        HealthStatManager healthStatManager;
        StaminaStatManager staminaStatManager;
        EquipmentGraphicsHandler equipmentGraphicsHandler;

        IMGUIContainer itemIcon;
        Label itemCounter;
        Label quickItemName;

        private void OnEnable()
        {
            healthStatManager = FindObjectOfType<HealthStatManager>(true);
            staminaStatManager = FindObjectOfType<StaminaStatManager>(true);
            equipmentGraphicsHandler = FindObjectOfType<EquipmentGraphicsHandler>(true);

            this.root = this.uIDocument.rootVisualElement;
            healthContainer = root.Q<VisualElement>("Health");
            healthFill = root.Q<VisualElement>("HealthFill");
            staminaContainer = root.Q<VisualElement>("Stamina");
            staminaFill = root.Q<VisualElement>("StaminaFill");

            itemIcon = root.Q<IMGUIContainer>("ItemIcon");
            itemCounter = root.Q<Label>("ItemCounterLabel");
            quickItemName = root.Q<Label>("QuickItemName");

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
            healthContainer.style.width = healthContainerBaseWidth + ((Player.instance.vitality + equipmentGraphicsHandler.vitalityBonus) * containerMultiplierPerLevel);
            staminaContainer.style.width = staminaContainerBaseWidth + ((Player.instance.endurance + equipmentGraphicsHandler.enduranceBonus) * containerMultiplierPerLevel);

            float healthPercentage = Mathf.Clamp(
                (Player.instance.currentHealth * 100) / healthStatManager.GetMaxHealth(),
                0,
                100
            );

            Length formattedHp = new Length(healthPercentage, LengthUnit.Percent);
            this.healthFill.style.width = formattedHp;


            float staminaPercentage = Mathf.Clamp(
                (Player.instance.currentStamina * 100) / staminaStatManager.GetMaxStamina(),
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

            itemIcon.style.backgroundImage = null;
            itemCounter.text = "";
            itemCounter.style.display = DisplayStyle.None;
            quickItemName.text = "";

            if (Player.instance == null || Player.instance.favoriteItems == null || Player.instance.favoriteItems.Count <= 0) { return; }

            itemIcon.style.backgroundImage = new StyleBackground(Player.instance.favoriteItems[0].sprite);
            itemCounter.text = Player.instance.ownedItems.Find(x => x.item == Player.instance.favoriteItems[0]).amount.ToString();
            itemCounter.style.display = DisplayStyle.Flex;
            quickItemName.text = Player.instance.favoriteItems[0].name;

        }

    }

}