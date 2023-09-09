using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace AF
{

    public class ViewSettingsMenu : ViewMenu
    {
        public enum ActiveMenu { OPTIONS, CONTROLS }

        public ActiveMenu activeMenu = ActiveMenu.OPTIONS;

        #region Buttons
        Button optionMenu;
        Button controlsMenu;
        #endregion

        VisualElement optionsContainer;
        VisualElement controlsContainer;

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

            optionsContainer = root.Q<VisualElement>("OptionsMenu");
            controlsContainer = root.Q<VisualElement>("ControlsMenu");

            optionMenu = root.Q<Button>("Options");
            bool isEnglish = GamePreferences.instance.IsEnglish();

            optionMenu.text = isEnglish ? "Options" : "Opções";

            controlsMenu = root.Q<Button>("Controls");
            controlsMenu.text = isEnglish ? "See Controls" : "Ver Controlos";

            optionMenu.clicked += () =>
            {
                activeMenu = ActiveMenu.OPTIONS;
                RedrawUI();
            };
            controlsMenu.clicked += () =>
            {
                activeMenu = ActiveMenu.CONTROLS;
                RedrawUI();
            };
        }

        void RedrawUI()
        {
            optionMenu.style.opacity = .5f;
            optionsContainer.style.display = DisplayStyle.None;

            controlsMenu.style.opacity = .5f;
            controlsContainer.style.display = DisplayStyle.None;

            if (activeMenu == ActiveMenu.OPTIONS)
            {
                optionsContainer.style.display = DisplayStyle.Flex;
                optionMenu.style.opacity = 1.5f;

                viewComponent_GameSettings.TranslateSettingsUI(root);
            }
            else if (activeMenu == ActiveMenu.CONTROLS)
            {
                controlsContainer.style.display = DisplayStyle.Flex;
                controlsMenu.style.opacity = 1.5f;

                TranslateControlsUI();
            }
        }


        void TranslateControlsUI()
        {
            bool isEnglish = GamePreferences.instance.IsEnglish();

            root.Q<Label>("ControlsTitle").style.display = DisplayStyle.None;

            root.Q<VisualElement>("Walk").Q<Label>().text = isEnglish ? "Walk" : "Andar";
            root.Q<VisualElement>("LightAttack").Q<Label>().text = isEnglish ? "Light Attack" : "Atacar";
            root.Q<VisualElement>("HeavyAttack").Q<Label>().text = isEnglish ? "Heavy Attack" : "Atacar com Força";
            root.Q<VisualElement>("BlockAndParry").Q<Label>().text = isEnglish ? "Block / Parry" : "Bloquear / Ripostar";
            root.Q<VisualElement>("Sprint").Q<Label>().text = isEnglish ? "Sprint" : "Sprintar";
            root.Q<VisualElement>("Jump").Q<Label>().text = isEnglish ? "Jump" : "Saltar";
            root.Q<VisualElement>("DodgeRoll").Q<Label>().text = isEnglish ? "Dodge Roll" : "Esquivar";
            root.Q<VisualElement>("Interact").Q<Label>().text = isEnglish ? "Interact" : "Interagir";
            root.Q<VisualElement>("UseFavoriteItem").Q<Label>().text = isEnglish ? "Use Favorite Item" : "Usar Item Favorito";
            root.Q<VisualElement>("SwitchFavoriteItem").Q<Label>().text = isEnglish ? "Switch Favorite Item" : "Trocar Item Favorito";
            root.Q<VisualElement>("OpenMenu").Q<Label>().text = isEnglish ? "Open Menu" : "Abrir Menu";
            root.Q<VisualElement>("ToggleWalkAndRun").Q<Label>().text = isEnglish ? "Toggle Walk / Run" : "Alternar Andar / Correr";
                
            root.Q<Label>("WASDKeys").text = isEnglish ? "WASD Keys" : "Teclas WASD";
            root.Q<Label>("LeftMouseButton").text = isEnglish ? "Left Mouse Button" : "Botão Esquerdo Rato";
            root.Q<Label>("RightMouseButton").text = isEnglish ? "Right Mouse Button" : "Botão Direito Rato";
            root.Q<Label>("Spacebar").text = isEnglish ? "Spacebar" : "Barra de Espaços";

        }

    }
}
