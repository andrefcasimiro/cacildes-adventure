using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace AF
{

    public class ViewObjectivesMenu : ViewMenu
    {
        [System.Serializable]
        public class VideoTutorial
        {
            public LocalizedText title;
            public EventPage eventPage;
        }

        public enum ActiveMenu { QUESTLOG, TUTORIALS, ACHIEVEMENTS, CONTROLS, VIDEO_TUTORIALS }

        public ActiveMenu activeMenu = ActiveMenu.QUESTLOG;

        #region Buttons
        Button questlogMenu;
        Button tutorialsMenu;
        Button achievementsMenu;
        Button controlsMenu;
        Button videoTutorialsMenu;
        #endregion

        VisualElement questlogContainer;
        VisualElement tutorialsContainer;
        VisualElement achievementsContainer;
        VisualElement controlsContainer;
        VisualElement videoTutorialsContainer;

        public VisualTreeAsset questPrefab;

        public Font vecnaFont;

        [Header("Tutorial Videos")]
        public VideoTutorial[] tutorialVideos;

        public CursorManager cursorManager;

        EventPage currentVideoTutorialEventPage;

        protected override void OnEnable()
        {
            base.OnEnable();


            SetupRefs();
            RedrawUI();
        }

        void SetupRefs()
        {

            questlogContainer = root.Q<VisualElement>("QuestlogContainer");
            achievementsContainer = root.Q<VisualElement>("AchievementsContainer");
            tutorialsContainer = root.Q<VisualElement>("TutorialsContainer");
            controlsContainer = root.Q<VisualElement>("ControlsMenu");
            videoTutorialsContainer = root.Q<VisualElement>("VideoTutorialsContainer");

            questlogMenu = root.Q<Button>("Questlog");
            bool isEnglish = GamePreferences.instance.IsEnglish();

            questlogMenu.text = isEnglish ? "Quests" : "Tarefas";
            questlogMenu.focusable = false;
            questlogMenu.clicked += () =>
            {
                Soundbank.instance.PlayUIHover();

                activeMenu = ActiveMenu.QUESTLOG;
                RedrawUI();
            };

            tutorialsMenu = root.Q<Button>("Guides");
            tutorialsMenu.text = isEnglish ? "Game Guide" : "Guia do Jogo";
            tutorialsMenu.focusable = false;
            tutorialsMenu.clicked += () =>
            {
                Soundbank.instance.PlayUIHover();
                activeMenu = ActiveMenu.TUTORIALS;
                RedrawUI();
            };

            achievementsMenu = root.Q<Button>("Achievements");
            achievementsMenu.text = isEnglish ? "See Achievements" : "Ver Conquistas";
            achievementsMenu.focusable = false;
            achievementsMenu.clicked += () =>
            {
                Soundbank.instance.PlayUIHover();
                activeMenu = ActiveMenu.ACHIEVEMENTS;
                RedrawUI();
            };

            controlsMenu = root.Q<Button>("SeeControls");
            controlsMenu.text = isEnglish ? "Controls" : "Controlos";
            controlsMenu.focusable = false;
            controlsMenu.clicked += () =>
            {
                Soundbank.instance.PlayUIHover();
                activeMenu = ActiveMenu.CONTROLS;
                RedrawUI();
            };

            videoTutorialsMenu = root.Q<Button>("Video_Tutorials");
            videoTutorialsMenu.text = isEnglish ? "Video Tutorials" : "Vídeos-tutoriais";
            videoTutorialsMenu.focusable = false;
            videoTutorialsMenu.clicked += () =>
            {
                Soundbank.instance.PlayUIHover();
                activeMenu = ActiveMenu.VIDEO_TUTORIALS;
                RedrawUI();
            };

        }

        void RedrawUI()
        {
            questlogMenu.style.opacity = .5f;
            questlogContainer.style.display = DisplayStyle.None;

            tutorialsMenu.style.opacity = .5f;
            tutorialsContainer.style.display = DisplayStyle.None;

            achievementsMenu.style.opacity = .5f;
            achievementsContainer.style.display = DisplayStyle.None;

            controlsMenu.style.opacity = .5f;
            controlsContainer.style.display = DisplayStyle.None;

            videoTutorialsMenu.style.opacity = .5f;
            videoTutorialsContainer.style.display = DisplayStyle.None;

            if (activeMenu == ActiveMenu.QUESTLOG)
            {
                questlogContainer.style.display = DisplayStyle.Flex;
                questlogMenu.style.opacity = 1.5f;

                DrawQuestsMenu();
            }
            else if (activeMenu == ActiveMenu.ACHIEVEMENTS)
            {
                achievementsContainer.style.display = DisplayStyle.Flex;
                achievementsMenu.style.opacity = 1.5f;
            }
            else if (activeMenu == ActiveMenu.CONTROLS)
            {
                controlsContainer.style.display = DisplayStyle.Flex;
                controlsMenu.style.opacity = 1.5f;

                TranslateControlsUI();
            }
            else if (activeMenu == ActiveMenu.VIDEO_TUTORIALS)
            {
                videoTutorialsContainer.style.display = DisplayStyle.Flex;
                videoTutorialsMenu.style.opacity = 1.5f;

                PopulateTutorialVideos();
            }
        }

        void DrawQuestsMenu()
        {
            bool isEnglish = GamePreferences.instance.IsEnglish();

            var missionFoldout = questlogContainer.Q<Foldout>("MainMissionFoldout");
            missionFoldout.focusable = false;

            var scrollView = missionFoldout.Q<ScrollView>();
            scrollView.Clear();

            missionFoldout.text = isEnglish ? "Main Mission" : "Missão Principal";

            var objectives = QuestManager.instance.GetMainQuestCurrentAndPastObjectives();
            objectives.Reverse();

            bool hasHighlightedObjective = false;
            foreach (var quest in objectives)
            {
                VisualElement clone = questPrefab.CloneTree();

                if (!hasHighlightedObjective)
                {
                    // Is Last
                    clone.style.opacity = 1;
                    hasHighlightedObjective = true;
                }
                else
                {
                    clone.style.opacity = 0.25f;
                }
                clone.Q<VisualElement>("Icon").style.backgroundImage = new StyleBackground(quest.icon);
                clone.Q<Label>("Description").text = quest.objective.GetText();
                clone.focusable = false;
                scrollView.Add(clone);
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

        public void PopulateTutorialVideos()
        {
            videoTutorialsContainer.Q<ScrollView>().Clear();

            foreach (var tutorial in tutorialVideos)
            {
                var btn = new Button();
                btn.AddToClassList("button");
                btn.style.color = Color.white;
                btn.focusable = false;

                btn.clicked += () =>
                {
                    currentVideoTutorialEventPage = tutorial.eventPage;
                    tutorial.eventPage._ForceBeginEvent();
                };

                btn.style.paddingBottom = 2;
                btn.style.paddingLeft = 2;
                btn.style.paddingRight = 2;
                btn.style.paddingTop = 2;
                btn.text = tutorial.title.GetText();
                btn.style.unityFont = vecnaFont;
                btn.style.fontSize = 16;
                btn.style.unityFontStyleAndWeight = FontStyle.Bold;
                btn.style.unityFontDefinition = new StyleFontDefinition(vecnaFont);
                btn.style.marginBottom = 4;

                videoTutorialsContainer.Q<ScrollView>().Add(btn);
            }
        }

        void OnDisable()
        {
            if (currentVideoTutorialEventPage != null)
            {
                currentVideoTutorialEventPage.StopEvent();
            }
        }


        private void Update()
        {
            if (UnityEngine.Cursor.visible == false)
            {
                cursorManager.ShowCursor();
            }
        }
    }
}
