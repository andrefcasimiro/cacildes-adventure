using System.Linq;
using AF.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class ViewSettingsMenu : ViewMenu
    {
        VisualElement optionsContainer;
        ViewComponent_GameSettings viewComponent_GameSettings => GetComponent<ViewComponent_GameSettings>();

        Button saveProgress, exitButton, newGamePlusButton;
        public const string saveGameLabel = "SaveGame";
        public const string exitGameLabel = "ExitGame";
        public const string newGamePlusLabel = "NewGamePlus";


        [Header("Quest To Allow New Game Plus")]
        public QuestParent questParentToAllowNewGamePlus;
        public int[] rangeOfQuestToAllowNewGamePlus;

        [Header("Stats")]
        public IntStat beginsNewGamePlusStat;

        protected override void OnEnable()
        {
            base.OnEnable();

            SetupRefs();
        }

        void SetupRefs()
        {
            viewComponent_GameSettings.SetupRefs(root);
            optionsContainer = root.Q<VisualElement>("OptionsMenu");
            optionsContainer.style.display = DisplayStyle.Flex;

            saveProgress = root.Q<Button>(saveGameLabel);
            exitButton = root.Q<Button>(exitGameLabel);
            newGamePlusButton = root.Q<Button>(newGamePlusLabel);
            saveProgress.SetEnabled(saveManager.CanSave());

            root.Q<Label>("CurrentNewGameCounter").text = " " + gameSession.currentGameIteration;

            UIUtils.SetupButton(saveProgress, () =>
            {
                soundbank.PlaySound(soundbank.uiHover);

                saveManager.SaveGameData(menuManager.screenshotBeforeOpeningMenu);

            }, soundbank);

            if (questParentToAllowNewGamePlus != null && rangeOfQuestToAllowNewGamePlus.Contains(questParentToAllowNewGamePlus.questProgress))
            {
                UIUtils.SetupButton(newGamePlusButton, () =>
                {
                    beginsNewGamePlusStat.UpdateStat();

                    soundbank.PlaySound(soundbank.uiHover);

                    fadeManager.FadeIn(1f, () =>
                    {
                        saveManager.ResetGameStateForNewGamePlusAndReturnToTitleScreen();
                    });

                }, soundbank);
                newGamePlusButton.style.display = DisplayStyle.Flex;
            }
            else
            {
                newGamePlusButton.style.display = DisplayStyle.None;
            }

            UIUtils.SetupButton(exitButton, () =>
            {
                soundbank.PlaySound(soundbank.uiHover);

                fadeManager.FadeIn(1f, () =>
                {
                    saveManager.ResetGameStateAndReturnToTitleScreen(false);
                });

            }, soundbank);

        }
    }
}
