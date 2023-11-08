using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace AF
{

    public class ViewSettingsMenu : ViewMenu
    {
        public enum ActiveMenu { OPTIONS, LOAD_GAME }

        public ActiveMenu activeMenu = ActiveMenu.OPTIONS;

        #region Buttons
        Button optionMenu;
        Button loadGame;
        #endregion

        VisualElement optionsContainer;

        ViewComponent_GameSettings viewComponent_GameSettings => GetComponent<ViewComponent_GameSettings>();


        protected override void OnEnable()
        {
            base.OnEnable();


            SetupRefs();
            RedrawUI();
        }


        void SetupRefs()
        {
            viewComponent_GameSettings.SetupRefs(root);

            bool isEnglish = GamePreferences.instance.IsEnglish();

            optionsContainer = root.Q<VisualElement>("OptionsMenu");

            optionMenu = root.Q<Button>("Options");
            optionMenu.text = isEnglish ? "Options" : "Opções";

            loadGame = root.Q<Button>("LoadGame");
            loadGame.text = isEnglish ? "Load Game" : "Carregar Jogo";


            optionMenu.clicked += () =>
            {
                activeMenu = ActiveMenu.OPTIONS;
                RedrawUI();
            };

            loadGame.clicked += () =>
            {
                FindAnyObjectByType<UIDocumentTitleScreenLoadMenu>(FindObjectsInactive.Include).gameObject.SetActive(true);
                menuManager.CloseMenu();

            };
        }

        void RedrawUI()
        {
            optionMenu.style.opacity = .5f;
            optionsContainer.style.display = DisplayStyle.None;

            if (activeMenu == ActiveMenu.OPTIONS)
            {
                optionsContainer.style.display = DisplayStyle.Flex;
                optionMenu.style.opacity = 1.5f;

                viewComponent_GameSettings.TranslateSettingsUI(root);
            }
        }



    }
}
