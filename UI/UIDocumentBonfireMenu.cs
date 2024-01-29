using System.Collections;
using AF.Bonfires;
using AF.Inventory;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentBonfireMenu : MonoBehaviour
    {
        [Header("UI Components")]
        public UIDocument uIDocument;
        public UIDocumentLevelUp uiDocumentLevelUp;
        public UIDocumentBonfireTravel uiDocumentTravel;
        public UIDocumentCraftScreen uIDocumentCraftScreen;

        [Header("Components")]
        public CursorManager cursorManager;
        public PlayerManager playerManager;
        public Soundbank soundbank;

        [Header("Databases")]
        public InventoryDatabase inventoryDatabase;
        public PlayerStatsDatabase playerStatsDatabase;

        [Header("References")]
        public Item blacksmithKit;
        public Item alchemyKit;
        public GameSession gameSession;

        [Header("UI Elements")]
        VisualElement root;
        Button levelUpButton, passTimeButton, exitBonfireButton, travelButton, upgradeWeapons, brewPotions;
        Label bonfireName, bonfireNameLabelUI, levelUpAvailableLabel, currentGoldAndRequiredLabel, goldAndRequiredForNextLevel;

        // Flags
        bool isPassingTime = false;
        float originalDaySpeed = 0f;
        bool hasEnoughForLevellingUp = false;

        Bonfire currentBonfire;

        private void Start()
        {
            originalDaySpeed = gameSession.daySpeed;
            gameObject.SetActive(false);
        }

        void SetupRefs()
        {
            this.root = uIDocument.rootVisualElement;

            bonfireName = root.Q<Label>("BonfireName");
            bonfireNameLabelUI = root.Q<Label>("BonfireNameLabel");
            levelUpAvailableLabel = root.Q<Label>("LevelUpAvailableLabel");
            currentGoldAndRequiredLabel = root.Q<Label>("CurrentGoldAndRequiredLabel");
            goldAndRequiredForNextLevel = root.Q<Label>("GoldAndRequiredFornextLevel");

            levelUpButton = root.Q<Button>("LevelUpButton");
            passTimeButton = root.Q<Button>("PassTimeButton");
            exitBonfireButton = root.Q<Button>("LeaveButton");
            travelButton = root.Q<Button>("TravelButton");
            upgradeWeapons = root.Q<Button>("UpgradeWeapons");
            brewPotions = root.Q<Button>("BrewPotions");
        }

        private void OnEnable()
        {
            SetupRefs();
            DrawUI();
        }

        public void SetCurrentBonfire(Bonfire bonfire)
        {
            this.currentBonfire = bonfire;
        }

        public void DrawUI()
        {
            cursorManager.ShowCursor();
            isPassingTime = false;

            if (currentBonfire != null)
            {
                bonfireName.text = currentBonfire.bonfireName;
            }

            bonfireNameLabelUI.text = "Bonfire Name";
            hasEnoughForLevellingUp = playerStatsDatabase.gold >= playerManager.playerLevelManager.GetRequiredExperienceForNextLevel();
            levelUpAvailableLabel.style.display = hasEnoughForLevellingUp ? DisplayStyle.Flex : DisplayStyle.None;

            SetupButtons();
        }

        void SetupButtons()
        {
            levelUpButton.text = "Level Up" + (hasEnoughForLevellingUp ? " *" : "");
            passTimeButton.text = "Wait 1 hour";
            travelButton.text = "Travel";
            upgradeWeapons.style.display = inventoryDatabase.HasItem(blacksmithKit) ? DisplayStyle.Flex : DisplayStyle.None;
            brewPotions.style.display = inventoryDatabase.HasItem(alchemyKit) ? DisplayStyle.Flex : DisplayStyle.None;
            SetButtonTexts();
            RegisterButtonCallbacks();

            exitBonfireButton.Focus();
        }

        void SetButtonTexts()
        {
            upgradeWeapons.text = "Upgrade Weapons";
            brewPotions.text = "Craft Items";
            exitBonfireButton.text = "Exit Bonfire";
            currentGoldAndRequiredLabel.text = "Your gold / Amount for next level";
            goldAndRequiredForNextLevel.text = $"{playerStatsDatabase.gold} / {playerManager.playerLevelManager.GetRequiredExperienceForNextLevel()}";
        }

        void RegisterButtonCallbacks()
        {
            UIUtils.SetupButton(levelUpButton, () =>
            {
                uiDocumentLevelUp.gameObject.SetActive(true);
                gameObject.SetActive(false);
            }, soundbank);

            UIUtils.SetupButton(passTimeButton, () =>
            {
                if (!isPassingTime)
                    StartCoroutine(MoveTime());
            }, soundbank);

            UIUtils.SetupButton(travelButton, () =>
            {
                uiDocumentTravel.gameObject.SetActive(true);
                this.gameObject.SetActive(false);
            }, soundbank);

            UIUtils.SetupButton(upgradeWeapons, () =>
            {
                uIDocumentCraftScreen.craftActivity = UIDocumentCraftScreen.CraftActivity.BLACKSMITH;
                uIDocumentCraftScreen.returnToBonfire = true;

                uIDocumentCraftScreen.gameObject.SetActive(true);
                this.gameObject.SetActive(false);
            }, soundbank);

            UIUtils.SetupButton(brewPotions, () =>
            {
                uIDocumentCraftScreen.craftActivity = UIDocumentCraftScreen.CraftActivity.ALCHEMY;
                uIDocumentCraftScreen.returnToBonfire = true;

                uIDocumentCraftScreen.gameObject.SetActive(true);
                this.gameObject.SetActive(false);
            }, soundbank);

            UIUtils.SetupButton(exitBonfireButton, () =>
            {
                ExitBonfire();
            }, soundbank);
        }

        void ExitBonfire()
        {
            cursorManager.HideCursor();
            gameSession.daySpeed = originalDaySpeed;
            currentBonfire.ExitBonfire();
            currentBonfire = null;
        }

        IEnumerator MoveTime()
        {
            if (!isPassingTime)
            {
                isPassingTime = true;
                var originalDaySpeed = gameSession.daySpeed;
                var targetHour = Mathf.Floor(gameSession.timeOfDay) + 1;

                if (targetHour > 23)
                {
                    gameSession.timeOfDay = 0;
                    targetHour = 0;
                }

                yield return null;
                gameSession.daySpeed = 2;

                yield return new WaitUntil(() => Mathf.FloorToInt(gameSession.timeOfDay) == Mathf.FloorToInt(targetHour));

                gameSession.daySpeed = originalDaySpeed;
                isPassingTime = false;
            }
        }
    }
}
