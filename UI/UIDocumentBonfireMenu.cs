using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentBonfireMenu : MonoBehaviour
    {
        public Bonfire bonfire;

        public UIDocumentLevelUp uiDocumentLevelUp;
        public UIDocumentBonfireTravel uiDocumentTravel;

        bool isPassingTime = false;
        float originalDaySpeed = 0f;

        [Header("Localization")]
        public LocalizedText bonfireNameLabel;
        public LocalizedText levelUpText;
        public LocalizedText passTimeText;
        public LocalizedText travelText;
        public LocalizedText exitBonfireText;
        
        CursorManager cursorManager;
        ThirdPersonController thirdPersonController;
        SceneSettings sceneSettings;
        DayNightManager dayNightManager;

        PlayerLevelManager playerLevelManager;
        PlayerInventory playerInventory;

        bool hasEnoughForLevellingUp = false;

        public Item blacksmithKit;
        public Item alchemyKit;



        private void Awake()
        {
            cursorManager = FindAnyObjectByType<CursorManager>(FindObjectsInactive.Include);
            thirdPersonController = FindAnyObjectByType<ThirdPersonController>(FindObjectsInactive.Include);
            playerLevelManager = thirdPersonController.GetComponent<PlayerLevelManager>();
            sceneSettings = FindAnyObjectByType<SceneSettings>(FindObjectsInactive.Include);
            dayNightManager = FindAnyObjectByType<DayNightManager>(FindObjectsInactive.Include);
            playerInventory = playerLevelManager.GetComponent<PlayerInventory>();
        }

        private void Start()
        {
            originalDaySpeed = Player.instance.daySpeed;
            gameObject.SetActive(false);

        }

        // THIS RUNS ONCE ON START. Maybe move gameObjectSetActive to Awake
        private void OnEnable()
        {
            isPassingTime = false;

            var root = GetComponent<UIDocument>().rootVisualElement;

            root.Q<Label>("BonfireName").text = bonfire.bonfireName.GetText();

            root.Q<Label>("BonfireNameLabel").text = bonfireNameLabel.GetText();

            hasEnoughForLevellingUp = Player.instance.currentGold >= playerLevelManager.GetRequiredExperienceForNextLevel();

            root.Q<Label>("LevelUpAvailableLabel").style.display = hasEnoughForLevellingUp ? DisplayStyle.Flex : DisplayStyle.None;

            #region Bonfire UI Buttons
            var levelUpButton = root.Q<Button>("LevelUpButton");
            var passTimeButton = root.Q<Button>("PassTimeButton");
            var exitBonfireButton = root.Q<Button>("LeaveButton");
            var travelButton = root.Q<Button>("TravelButton");
            var upgradeWeapons = root.Q<Button>("UpgradeWeapons");
            var brewPotions = root.Q<Button>("BrewPotions");
            upgradeWeapons.style.display = playerInventory.GetItemQuantity(blacksmithKit) <= 0 ? DisplayStyle.None : DisplayStyle.Flex;
            brewPotions.style.display = playerInventory.GetItemQuantity(alchemyKit) <= 0 ? DisplayStyle.None : DisplayStyle.Flex;

            levelUpButton.text = levelUpText.GetText();

            if (hasEnoughForLevellingUp)
            {
                levelUpButton.text += " *";
            }

            passTimeButton.text = passTimeText.GetText();
            travelButton.text = travelText.GetText();
            upgradeWeapons.text = GamePreferences.instance.IsEnglish() ? "Upgrade Weapons" : "Melhorar Armas";
            brewPotions.text = GamePreferences.instance.IsEnglish() ? "Brew Potions" : "Criar Poções";
            travelButton.text = travelText.GetText();
            exitBonfireButton.text = exitBonfireText.GetText();


            root.Q<Label>("CurrentGoldAndRequiredLabel").text = GamePreferences.instance.IsEnglish() ? "Your gold / Amount for next level" : "Moedas / Necessárias para próx. nível";
            root.Q<Label>("GoldAndRequiredFornextLevel").text = Player.instance.currentGold + " / " + playerLevelManager.GetRequiredExperienceForNextLevel();

            levelUpButton.RegisterCallback<ClickEvent>(ev =>
            {
                uiDocumentLevelUp.gameObject.SetActive(true);
                this.gameObject.SetActive(false);
            });

            passTimeButton.RegisterCallback<ClickEvent>(ev =>
            {
                if (isPassingTime)
                {
                    return;
                }

                StartCoroutine(MoveTime());
            });

            travelButton.RegisterCallback<ClickEvent>(ev =>
            {
                uiDocumentTravel.gameObject.SetActive(true);
                this.gameObject.SetActive(false);
            });


            upgradeWeapons.RegisterCallback<ClickEvent>(ev =>
            {
                var blackSmithScreen = FindAnyObjectByType<UIDocumentBlacksmithScreen>(FindObjectsInactive.Include);
                blackSmithScreen.returnToBonfire = true;

                blackSmithScreen.gameObject.SetActive(true);
                this.gameObject.SetActive(false);
            });
            brewPotions.RegisterCallback<ClickEvent>(ev =>
            {
                var alchemyCraftScreen = FindAnyObjectByType<UIDocumentAlchemyCraftScreen>(FindObjectsInactive.Include);
                alchemyCraftScreen.returnToBonfire = true;

                alchemyCraftScreen.gameObject.SetActive(true);
                this.gameObject.SetActive(false);
            });

            exitBonfireButton.RegisterCallback<ClickEvent>(ev =>
            {
                ExitBonfire();
            });
            #endregion

            cursorManager.ShowCursor();
        }

        void ExitBonfire()
        {

            Player.instance.daySpeed = originalDaySpeed;

            bonfire.ExitBonfire();
            StartCoroutine(DisableCursor());

            thirdPersonController.LockCameraPosition = false;
        }

        private void Update()
        {
            if (UnityEngine.Cursor.visible == false)
            {
                cursorManager.ShowCursor();
            }
        }

        IEnumerator DisableCursor()
        {
            yield return new WaitForSeconds(1f);
            cursorManager.HideCursor();
        }

        IEnumerator MoveTime()
        {
            if (!isPassingTime)
            {
                isPassingTime = true;

                bool isInteriorOriginal = sceneSettings.isInterior;

                sceneSettings.isInterior = false;
                dayNightManager.tick = true;
                var originalDaySpeed = Player.instance.daySpeed;

                var targetHour = Mathf.Floor(Player.instance.timeOfDay) + 1;

                if (targetHour > 23)
                {
                    Player.instance.timeOfDay = 0;
                    targetHour = 0;
                }

                yield return null;

                Player.instance.daySpeed = 2;

                yield return new WaitUntil(() => Mathf.FloorToInt(Player.instance.timeOfDay) == Mathf.FloorToInt(targetHour));

                Player.instance.daySpeed = originalDaySpeed;

                dayNightManager.tick = dayNightManager.TimePassageAllowed();
                sceneSettings.isInterior = isInteriorOriginal;

                isPassingTime = false;
            }

            yield return null;
        }
    }

}
