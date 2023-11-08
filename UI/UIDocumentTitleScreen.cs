using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentTitleScreen : MonoBehaviour
    {
        UIDocument document => GetComponent<UIDocument>();

        [Header("Components")]
        public TitleScreenManager titleScreenManager;
        public CursorManager cursorManager;
        public UIDocumentTitleScreenLoadMenu uIDocumentTitleScreenLoadMenu;
        public UIDocumentTitleScreenCredits uIDocumentTitleScreenCredits;
        public UIDocumentTitleScreenOptions uIDocumentTitleScreenOptions;
        public UIDocumentTitleScreenControls uIDocumentTitleScreenControls;
        public UIManager uiManager;

        [Header("Game Session")]
        public GameSession gameSession;

        VisualElement root;

        private void OnEnable()
        {
            root = document.rootVisualElement;

            var versionLabel = root.Q<Label>("Version");
            versionLabel.text = Application.version;

            Button newGameButton = root.Q<Button>("NewGameButton");

            Button continueButton = root.Q<Button>("ContinueButton");
            Button playTutorialButton = root.Q<Button>("PlayTutorialButton");
            Button optionsButton = root.Q<Button>("OptionsButton");
            Button controlsButton = root.Q<Button>("ControlsButton");
            Button creditsButton = root.Q<Button>("CreditsButton");
            Button exitButton = root.Q<Button>("ExitButton");
            Button btnTwitter = root.Q<Button>("btnTwitter");
            Button btnDiscord = root.Q<Button>("btnDiscord");

            uiManager.SetupButton(newGameButton, () =>
            {
                titleScreenManager.StartGame();
                gameObject.SetActive(false);
            });

            uiManager.SetupButton(continueButton, () =>
            {
                uIDocumentTitleScreenLoadMenu.gameObject.SetActive(true);
                gameObject.SetActive(false);
            });

            uiManager.SetupButton(playTutorialButton, () =>
            {
                Player.instance.LoadScene(6, true);
            });

            uiManager.SetupButton(creditsButton, () =>
            {
                uIDocumentTitleScreenCredits.gameObject.SetActive(true);
                gameObject.SetActive(false);
            });

            uiManager.SetupButton(optionsButton, () =>
            {
                uIDocumentTitleScreenOptions.gameObject.SetActive(true);
                gameObject.SetActive(false);
            });

            uiManager.SetupButton(controlsButton, () =>
            {
                uIDocumentTitleScreenControls.gameObject.SetActive(true);
                gameObject.SetActive(false);
            });

            uiManager.SetupButton(exitButton, () =>
            {
                Application.Quit();
            });

            uiManager.SetupButton(btnTwitter, () =>
            {
                Application.OpenURL("https://twitter.com/CacildesGame");
            });

            uiManager.SetupButton(btnDiscord, () =>
            {
                Application.OpenURL("https://discord.gg/JwnZMc27D2");
            });


            cursorManager.ShowCursor();

            // Delay the focus until the next frame, required as an hack for now
            Invoke(nameof(GiveFocus), 0f);
        }

        void GiveFocus()
        {
            Button newGameButton = root.Q<Button>("NewGameButton");

            newGameButton.Focus();
        }
    }
}
