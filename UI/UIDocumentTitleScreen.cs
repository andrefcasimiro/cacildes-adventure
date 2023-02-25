using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentTitleScreen : MonoBehaviour
    {
        [Header("Localization")]
        public LocalizedText gameTitle;
        public LocalizedText newGameText;
        public LocalizedText loadGameText;
        public LocalizedText playTutorialText;
        public LocalizedText controlsText;
        public LocalizedText creditsText;
        public LocalizedText exitGameText;

        UIDocument document => GetComponent<UIDocument>();
        TitleScreenManager titleScreenManager => GetComponentInParent<TitleScreenManager>();
        MenuManager menuManager;

        private void Awake()
        {
            Utils.ShowCursor();

            menuManager = FindObjectOfType<MenuManager>(true);
        }

        private void Start()
        {
            if (Player.instance.hasShownTitleScreen == false)
            {
                FindObjectOfType<UIDocumentPlayerHUDV2>(true).gameObject.SetActive(false);
            }
        }

        void TranslateText(VisualElement root)
        {
            root.Q<Label>("GameTitle").text = gameTitle.GetText();
            root.Q<Button>("NewGameButton").text = newGameText.GetText();
            root.Q<Button>("ContinueButton").text = loadGameText.GetText();
            root.Q<Button>("PlayTutorialButton").text = playTutorialText.GetText();
            root.Q<Button>("ControlsButton").text = controlsText.GetText();
            root.Q<Button>("CreditsButton").text = creditsText.GetText();
            root.Q<Button>("ExitButton").text = exitGameText.GetText();
        }

        private void OnEnable()
        {
            var root = document.rootVisualElement;

            root.Q<Label>("Version").text = Application.version;

            TranslateText(root);

            menuManager.SetupButton(
                root.Q<Button>("NewGameButton"),
                () =>
                {
                    FindObjectOfType<UIDocumentPlayerHUDV2>(true).gameObject.SetActive(true);

                    titleScreenManager.StartGame();

                    gameObject.SetActive(false);
                });
            menuManager.SetupButton(
                root.Q<Button>("ContinueButton"),
                () =>
                {
                    FindObjectOfType<UIDocumentTitleScreenLoadMenu>(true).gameObject.SetActive(true);
                    gameObject.SetActive(false);
                });
            menuManager.SetupButton(
                root.Q<Button>("PlayTutorialButton"),
                () =>
                {
                    Player.instance.LoadScene(6, true);
                });
            menuManager.SetupButton(
                root.Q<Button>("CreditsButton"),
                () =>
                {
                    FindObjectOfType<UIDocumentTitleScreenCredits>(true).gameObject.SetActive(true);
                    gameObject.SetActive(false);
                });
            menuManager.SetupButton(
                root.Q<Button>("ControlsButton"),
                () =>
                {
                    FindObjectOfType<UIDocumentTitleScreenControls>(true).gameObject.SetActive(true);
                    gameObject.SetActive(false);
                });
            menuManager.SetupButton(
                root.Q<Button>("ControlsButton"),
                () =>
                {
                    FindObjectOfType<UIDocumentTitleScreenControls>(true).gameObject.SetActive(true);
                    gameObject.SetActive(false);
                });
            menuManager.SetupButton(
                root.Q<Button>("ExitButton"),
                () =>
                {
                    Application.Quit();
                });
            menuManager.SetupButton(
                root.Q<Button>("btnBlogger"),
                () =>
                {
                    Application.OpenURL("https://cacildesadventure.blogspot.com/");
                });
            menuManager.SetupButton(
                root.Q<Button>("btnItchio"),
                () =>
                {
                    Application.OpenURL("https://andrefcasimiro.itch.io/cacildes-adventure");
                });
            menuManager.SetupButton(
                root.Q<Button>("btnTwitter"),
                () =>
                {
                    Application.OpenURL("https://twitter.com/CacildesGame");
                });
        }
    }
}
