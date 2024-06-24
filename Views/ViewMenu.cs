using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.Localization;

namespace AF.UI
{
    public class ViewMenu : MonoBehaviour
    {
        [HideInInspector]
        public VisualElement root;

        public const string EQUIPMENT_BUTTON = "EquipmentButton";
        public const string OBJECTIVES_BUTTON = "ObjectivesButton";
        public const string OPTIONS_BUTTON = "OptionsGameButton";
        public const string LABEL_DESCRIPTOR = "Descriptor";

        Button equipmentButton;
        Button objectivesButton;
        Button optionsButton;
        Label descriptor;

        // Components
        protected MenuManager menuManager;

        [Header("Components")]
        public CursorManager cursorManager;
        public Soundbank soundbank;
        public FadeManager fadeManager;
        public SaveManager saveManager;
        public GameSession gameSession;

        [Header("Localization")]
        public LocalizedString equipment_LocalizedString;
        public LocalizedString quests_LocalizedString;
        public LocalizedString settings_LocalizedString;

        protected virtual void OnEnable()
        {
            SetupRefs();

            equipmentButton.RemoveFromClassList("active");
            objectivesButton.RemoveFromClassList("active");
            optionsButton.RemoveFromClassList("active");

            switch (menuManager.viewMenuIndex)
            {
                case 0:
                    equipmentButton.AddToClassList("active");
                    descriptor.text = equipment_LocalizedString.GetLocalizedString();
                    break;
                case 1:
                    objectivesButton.AddToClassList("active");
                    descriptor.text = quests_LocalizedString.GetLocalizedString();
                    break;
                case 2:
                    optionsButton.AddToClassList("active");
                    descriptor.text = settings_LocalizedString.GetLocalizedString();
                    break;
                default:
                    return;
            }

        }

        private void Update()
        {
            if (UnityEngine.Cursor.visible == false)
            {
                cursorManager.ShowCursor();
            }
        }

        void SetupRefs()
        {
            menuManager = menuManager != null ? menuManager : FindAnyObjectByType<MenuManager>(FindObjectsInactive.Include);
            root = GetComponent<UIDocument>().rootVisualElement;

            if (root != null && menuManager.hasPlayedFadeIn == false)
            {
                menuManager.hasPlayedFadeIn = true;

                soundbank.PlaySound(soundbank.mainMenuOpen);

                DOTween.To(
                      () => root.contentContainer.style.opacity.value,
                      (value) => root.contentContainer.style.opacity = value,
                      1,
                      .25f
                );
            }
            descriptor = root.Q<Label>(LABEL_DESCRIPTOR);

            equipmentButton = root.Q<Button>(EQUIPMENT_BUTTON);
            UIUtils.SetupButton(equipmentButton, () =>
            {
                soundbank.PlaySound(soundbank.uiHover);
                menuManager.viewMenuIndex = 0;
                menuManager.SetMenuView();
            }, soundbank);

            objectivesButton = root.Q<Button>(OBJECTIVES_BUTTON);
            UIUtils.SetupButton(objectivesButton, () =>
            {

                soundbank.PlaySound(soundbank.uiHover);
                menuManager.viewMenuIndex = 1;
                menuManager.SetMenuView();
            }, soundbank);

            optionsButton = root.Q<Button>(OPTIONS_BUTTON);
            UIUtils.SetupButton(optionsButton, () =>
            {
                soundbank.PlaySound(soundbank.uiHover);
                menuManager.viewMenuIndex = 2;
                menuManager.SetMenuView();
            }, soundbank);
        }
    }
}
