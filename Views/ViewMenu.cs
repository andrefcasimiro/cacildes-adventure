using UnityEngine;
using UnityEngine.UIElements;

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
        MenuManager menuManager;

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

        private void OnDisable()
        {
        }


        void SetupRefs()
        {
            menuManager = menuManager != null ? menuManager : FindAnyObjectByType<MenuManager>(FindObjectsInactive.Include);
            root = GetComponent<UIDocument>().rootVisualElement;

            equipmentButton = root.Q<Button>(EQUIPMENT_BUTTON);
            equipmentButton.clicked += () => { menuManager.SetMenuView(ActiveMenu.EQUIPMENT); };

            objectivesButton = root.Q<Button>(OBJECTIVES_BUTTON);
            objectivesButton.clicked += () => { menuManager.SetMenuView(ActiveMenu.OBJECTIVES); };

            optionsButton = root.Q<Button>(OPTIONS_BUTTON);
            optionsButton.clicked += () => { menuManager.SetMenuView(ActiveMenu.OPTIONS); };

            exitButton = root.Q<Button>(EXIT_BUTTON);

            exitButton.clicked += () => {
                Application.Quit();
            };

        }
    }
}
