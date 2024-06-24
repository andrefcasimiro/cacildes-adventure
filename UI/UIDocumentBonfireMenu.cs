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
        Label bonfireName, bonfireNameLabelUI, canLevelUpIndicator, currentGoldAndRequiredLabel, goldAndRequiredForNextLevel;

        // Flags
        bool isPassingTime = false;
        float originalDaySpeed = 0f;
        bool hasEnoughForLevellingUp = false;

        Bonfire currentBonfire;
        bool canEscape = false;

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
            canLevelUpIndicator = root.Q<Label>("CanLevelUpIndicator");
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
            canEscape = false;
            SetupRefs();
            DrawUI();
            Invoke(nameof(ResetCanEscapeFlag), 0.5f);
        }

        void ResetCanEscapeFlag()
        {
            canEscape = true;
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
                bonfireName.text = currentBonfire.GetBonfireName();
            }

            hasEnoughForLevellingUp = playerStatsDatabase.gold >= playerManager.playerLevelManager.GetRequiredExperienceForNextLevel();

            SetupButtons();
        }

        void SetupButtons()
        {
            canLevelUpIndicator.text = hasEnoughForLevellingUp ? " *" : "";

            upgradeWeapons.style.display = inventoryDatabase.HasItem(blacksmithKit) ? DisplayStyle.Flex : DisplayStyle.None;
            brewPotions.style.display = inventoryDatabase.HasItem(alchemyKit) ? DisplayStyle.Flex : DisplayStyle.None;
            SetButtonTexts();
            RegisterButtonCallbacks();

            travelButton.style.display = (currentBonfire != null && currentBonfire.canUseTravelToOtherMaps) ? DisplayStyle.Flex : DisplayStyle.None;

            exitBonfireButton.Focus();
        }

        void SetButtonTexts()
        {
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

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnClose()
        {
            if (this.isActiveAndEnabled == false || canEscape == false)
            {
                return;
            }

            if (uiDocumentLevelUp.isActiveAndEnabled == false
                && uiDocumentTravel.isActiveAndEnabled == false
                && uIDocumentCraftScreen.isActiveAndEnabled == false
                )
            {
                ExitBonfire();
            }
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
