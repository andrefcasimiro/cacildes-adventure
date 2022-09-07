using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    [RequireComponent(typeof(UIDocument))]
    public class UIDocumentPlayerHUD: UIDocumentBase
    {

        public IMGUIContainer hpbar;
        public IMGUIContainer mpBar;
        public IMGUIContainer staminaBar;

        public IMGUIContainer favoriteIcon;
        public Label favoriteIconQuantity;
        public IMGUIContainer spellsIcon;

        public VisualElement statusContainer;
        public IMGUIContainer consumableIcon;
        public Label statusDescription;
        public Label statusTimer;

        public Sprite defaultEmptyFavoriteIcon;

        protected override void Start()
        {
            base.Start();

            hpbar = root.Q<IMGUIContainer>("hp-bar");
            mpBar = root.Q<IMGUIContainer>("mp-bar");
            staminaBar = root.Q<IMGUIContainer>("stamina-bar");
            favoriteIcon = root.Q<IMGUIContainer>("favorite-icon");
            favoriteIconQuantity = favoriteIcon.Q<Label>("quantity");
            spellsIcon = root.Q<IMGUIContainer>("spells-icon");
            statusContainer = root.Q<VisualElement>("status");
            consumableIcon = root.Q<IMGUIContainer>("consumable-icon");
            statusDescription = root.Q<Label>("StatusDescription");
            statusTimer = root.Q<Label>("StatusTimer");

            UpdateQuickItems();
        }

        public void Update()
        {
            UpdateHealthBar();
            UpdateStaminaBar();
            UpdateConsumableStatus();
        }

        void UpdateHealthBar()
        {
            if (this.hpbar == null)
            {
                return;
            }

            float maxHealthPoints = PlayerStatsManager.instance.GetMaxHealthPoints();
            float currentHealth = PlayerStatsManager.instance.GetCurrentHealth();

            float healthPercentage = Mathf.Clamp(
                (currentHealth * 100) / maxHealthPoints,
                0,
                100
            );

            Length formattedHp = new Length(healthPercentage, LengthUnit.Percent);
            this.hpbar.style.width = formattedHp;
        }

        void UpdateStaminaBar()
        {
            if (this.staminaBar == null)
            {
                return;
            }

            float maxStaminaPoints = PlayerStatsManager.instance.GetMaxStaminaPoints();

            float staminaPercentage = Mathf.Clamp(
                (PlayerStatsManager.instance.currentStamina * 100) / maxStaminaPoints,
                0,
                100
            );

            Length formattedStamina = new Length(staminaPercentage, LengthUnit.Percent);
            this.staminaBar.style.width = formattedStamina;
        }

        public void UpdateQuickItems()
        {
            if (PlayerInventoryManager.instance.currentFavoriteItems.Count <= 0)
            {
                this.favoriteIconQuantity.AddToClassList("hide");

                this.favoriteIcon.style.backgroundImage = new StyleBackground(defaultEmptyFavoriteIcon);

                return;
            }

            this.favoriteIconQuantity.RemoveFromClassList("hide");

            Item currentFavoriteItem = PlayerInventoryManager.instance.currentFavoriteItems[0];

            this.favoriteIcon.style.backgroundImage = new StyleBackground(currentFavoriteItem.sprite);
            
            ItemEntry item = PlayerInventoryManager.instance.currentItems.Find(item => item.item.name == currentFavoriteItem.name);
            if (item != null)
            {
                this.favoriteIconQuantity.text = item.amount.ToString();
            }

        }

        void UpdateConsumableStatus()
        {
            if (PlayerStatsManager.instance.appliedConsumables.Count <= 0)
            {
                this.statusContainer.AddToClassList("hide");
                return;
            }

            this.statusContainer.RemoveFromClassList("hide");

            foreach (AppliedConsumable appliedConsumable in PlayerStatsManager.instance.appliedConsumables)
            {
                this.consumableIcon.style.backgroundImage = new StyleBackground(appliedConsumable.consumable.sprite);

                this.statusDescription.text = appliedConsumable.consumable.smallEffectDescription;

                System.TimeSpan time = System.TimeSpan.FromSeconds(appliedConsumable.currentDuration);
                System.DateTime dateTime = System.DateTime.Today.Add(time);
                string displayTime = dateTime.ToString("mm:ss");

                this.statusTimer.text = displayTime;
            }

        }

    }

}
