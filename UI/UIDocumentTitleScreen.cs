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
        public UIDocumentTitleScreenCredits uIDocumentTitleScreenCredits;
        public UIDocumentTitleScreenOptions uIDocumentTitleScreenOptions;
        public UIDocumentTitleScreenSaveFiles uIDocumentTitleScreenSaveFiles;
        public Soundbank soundbank;
        public SaveManager saveManager;

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
            Button loadGameButton = root.Q<Button>("LoadGameButton");
            Button playTutorialButton = root.Q<Button>("PlayTutorialButton");
            Button optionsButton = root.Q<Button>("OptionsButton");
            Button controlsButton = root.Q<Button>("ControlsButton");
            Button creditsButton = root.Q<Button>("CreditsButton");
            Button exitButton = root.Q<Button>("ExitButton");
            Button btnGithub = root.Q<Button>("btnTwitter");
            Button btnDiscord = root.Q<Button>("btnDiscord");

            UIUtils.SetupButton(newGameButton, () =>
            {
                saveManager.ResetGameState(false);
                titleScreenManager.StartGame();
                gameObject.SetActive(false);
            }, soundbank);

            continueButton.SetEnabled(saveManager.HasSavedGame());

            UIUtils.SetupButton(continueButton, () =>
            {
                saveManager.LoadLastSavedGame(false);
                gameObject.SetActive(false);
            }, soundbank);

            UIUtils.SetupButton(loadGameButton, () =>
            {
                uIDocumentTitleScreenSaveFiles.gameObject.SetActive(true);
                gameObject.SetActive(false);
            }, soundbank);

            UIUtils.SetupButton(playTutorialButton, () =>
            {
                //Player.instance.LoadScene(6, true);
            }, soundbank);

            UIUtils.SetupButton(creditsButton, () =>
            {
                uIDocumentTitleScreenCredits.gameObject.SetActive(true);
                gameObject.SetActive(false);
            }, soundbank);

            UIUtils.SetupButton(optionsButton, () =>
            {
                uIDocumentTitleScreenOptions.gameObject.SetActive(true);
                gameObject.SetActive(false);
            }, soundbank);

            UIUtils.SetupButton(exitButton, () =>
            {
                Application.Quit();
            }, soundbank);

            UIUtils.SetupButton(btnGithub, () =>
            {
                Application.OpenURL("https://github.com/andrefcasimiro/cacildes-adventure");
            }, soundbank);

            UIUtils.SetupButton(btnDiscord, () =>
            {
                Application.OpenURL("https://discord.gg/JwnZMc27D2");
            }, soundbank);


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
