using AF.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace AF
{
    public class ViewSettingsMenu : ViewMenu
    {
        VisualElement optionsContainer;
        ViewComponent_GameSettings viewComponent_GameSettings => GetComponent<ViewComponent_GameSettings>();

        Button saveProgress, exitButton;
        public const string saveGameLabel = "SaveGame";
        public const string exitGameLabel = "ExitGame";

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
            saveProgress.SetEnabled(saveManager.CanSave());

            UIUtils.SetupButton(saveProgress, () =>
            {
                soundbank.PlaySound(soundbank.uiHover);

                saveManager.SaveGameData();

            }, soundbank);

            UIUtils.SetupButton(exitButton, () =>
            {
                soundbank.PlaySound(soundbank.uiHover);

                fadeManager.FadeIn(1f, () =>
                {
                    saveManager.ResetGameState();
                    gameSession.gameState = GameSession.GameState.INITIALIZED;
                    SceneManager.LoadScene(0);
                });

            }, soundbank);
        }
    }
}
