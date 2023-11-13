using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

namespace AF
{
    public class ViewMenu : MonoBehaviour
    {
        [HideInInspector]
        public VisualElement root;

        public const string EQUIPMENT_BUTTON = "EquipmentButton";
        public const string OBJECTIVES_BUTTON = "ObjectivesButton";
        public const string OPTIONS_BUTTON = "OptionsGameButton";
        public const string EXIT_BUTTON = "ExitGameButton";

        Button equipmentButton;
        Button objectivesButton;
        Button optionsButton;
        Button exitButton;

        // Components
        protected MenuManager menuManager;
        public CursorManager cursorManager;

        private void OnApplicationFocus(bool focus)
        {
            if (focus && root != null)
            {
                root.Focus();
            }
        }

        protected virtual void OnEnable()
        {
            SetupRefs();

            equipmentButton.RemoveFromClassList("active");
            objectivesButton.RemoveFromClassList("active");
            optionsButton.RemoveFromClassList("active");
            exitButton.RemoveFromClassList("active");

            switch (menuManager.activeMenu)
            {
                case ActiveMenu.EQUIPMENT:
                    equipmentButton.AddToClassList("active");
                    break;
                case ActiveMenu.OBJECTIVES:
                    objectivesButton.AddToClassList("active");
                    break;
                case ActiveMenu.EXIT_GAME:
                    exitButton.AddToClassList("active");
                    break;
                case ActiveMenu.OPTIONS:
                    optionsButton.AddToClassList("active");
                    break;
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

                Soundbank.instance.PlayMainMenuOpen();

                DOTween.To(
                      () => root.contentContainer.style.opacity.value,
                      (value) => root.contentContainer.style.opacity = value,
                      1,
                      .25f
                );

            }


            equipmentButton = root.Q<Button>(EQUIPMENT_BUTTON);
            equipmentButton.clicked += () =>
            {
                Soundbank.instance.PlayUIHover();
                menuManager.SetMenuView(ActiveMenu.EQUIPMENT);
            };

            objectivesButton = root.Q<Button>(OBJECTIVES_BUTTON);
            objectivesButton.clicked += () =>
            {
                Soundbank.instance.PlayUIHover();
                menuManager.SetMenuView(ActiveMenu.OBJECTIVES);
            };

            optionsButton = root.Q<Button>(OPTIONS_BUTTON);
            optionsButton.clicked += () =>
            {
                Soundbank.instance.PlayUIHover();
                menuManager.SetMenuView(ActiveMenu.OPTIONS);
            };

            exitButton = root.Q<Button>(EXIT_BUTTON);

            exitButton.clicked += () =>
            {
                Application.Quit();
            };

        }
    }
}
