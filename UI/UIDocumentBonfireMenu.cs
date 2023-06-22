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

        private void Awake()
        {
            cursorManager = FindObjectOfType<CursorManager>(true);
        }
        
        private void Start()
        {
            originalDaySpeed = Player.instance.daySpeed;
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            isPassingTime = false;

            var root = GetComponent<UIDocument>().rootVisualElement;

            root.Q<Label>("BonfireName").text = bonfire.bonfireName.GetText();

            root.Q<Label>("BonfireNameLabel").text = bonfireNameLabel.GetText();

            #region Bonfire UI Buttons
            var levelUpButton = root.Q<Button>("LevelUpButton");
            var passTimeButton = root.Q<Button>("PassTimeButton");
            var exitBonfireButton = root.Q<Button>("LeaveButton");
            var travelButton = root.Q<Button>("TravelButton");

            levelUpButton.text = levelUpText.GetText();
            passTimeButton.text = passTimeText.GetText();
            travelButton.text = travelText.GetText();
            exitBonfireButton.text = exitBonfireText.GetText();

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

            exitBonfireButton.RegisterCallback<ClickEvent>(ev =>
            {
                Player.instance.daySpeed = originalDaySpeed;

                bonfire.ExitBonfire();
                FindObjectOfType<GamepadCursor>(true).gameObject.SetActive(false);
            });
            #endregion

            cursorManager.ShowCursor();

            FindObjectOfType<GamepadCursor>(true).gameObject.SetActive(true);
        }

        IEnumerator MoveTime()
        {
            if (!isPassingTime)
            {
                isPassingTime = true;

                bool isInteriorOriginal = FindObjectOfType<SceneSettings>(true).isInterior;

                FindObjectOfType<SceneSettings>(true).isInterior = false;
                FindObjectOfType<DayNightManager>(true).tick = true;
                var originalDaySpeed = Player.instance.daySpeed;

                var targetHour = Mathf.Floor(Player.instance.timeOfDay) + 1;

                if (targetHour > 23)
                {
                    Player.instance.timeOfDay = 0;
                    targetHour = 0;
                }

                yield return null;

                Player.instance.daySpeed = 2;

                yield return new WaitUntil(() => Mathf.Floor(Player.instance.timeOfDay) == targetHour);

                Player.instance.daySpeed = originalDaySpeed;

                FindObjectOfType<DayNightManager>(true).tick = FindObjectOfType<DayNightManager>(true).TimePassageAllowed();
                FindObjectOfType<SceneSettings>(true).isInterior = isInteriorOriginal;

                isPassingTime = false;
            }

            yield return null;
        }
    }

}
