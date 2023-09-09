using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace AF
{

    public class ViewObjectivesMenu : ViewMenu
    {
        public enum ActiveMenu { QUESTLOG, ACHIEVEMENTS }

        public ActiveMenu activeMenu = ActiveMenu.QUESTLOG;

        #region Buttons
        Button questlogMenu;
        Button achievementsMenu;
        #endregion

        VisualElement questlogContainer;
        VisualElement achievementsContainer;

        public VisualTreeAsset questPrefab;


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

            questlogMenu = root.Q<Button>("Questlog");
            bool isEnglish = GamePreferences.instance.IsEnglish();

            questlogMenu.text = isEnglish ? "Things to do" : "Coisas para fazer";

            questlogMenu.clicked += () =>
            {
                activeMenu = ActiveMenu.QUESTLOG;
                RedrawUI();
            };

            achievementsMenu = root.Q<Button>("Achievements");
            achievementsMenu.text = isEnglish ? "See Achievements" : "Ver Conquistas";

            achievementsMenu.clicked += () =>
            {
                activeMenu = ActiveMenu.ACHIEVEMENTS;
                RedrawUI();
            };
        }

        void RedrawUI()
        {
            questlogMenu.style.opacity = .5f;
            questlogContainer.style.display = DisplayStyle.None;

            achievementsMenu.style.opacity = .5f;
            achievementsContainer.style.display = DisplayStyle.None;

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
        }

        void DrawQuestsMenu()
        {
            bool isEnglish = GamePreferences.instance.IsEnglish();

            var missionFoldout = questlogContainer.Q<Foldout>("MainMissionFoldout");

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

                scrollView.Add(clone);
            }

        }


    }
}
