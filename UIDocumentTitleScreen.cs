using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{

    public class UIDocumentTitleScreen : MonoBehaviour
    {
        UIDocument uIDocument => GetComponent<UIDocument>();

        TitleScreenManager titleScreenManager => GetComponentInParent<TitleScreenManager>();

        MenuManager menuManager;
        bool hasFocused = false;

        private void Awake()
        {
            menuManager = FindObjectOfType<MenuManager>(true);
        }

        private void Start()
        {
            if (Player.instance.hasShownTitleScreen == false)
            {
                FindObjectOfType<UIDocumentPlayerHUDV2>(true).gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            menuManager.SetupButton(
                root.Q<Button>("NewGameButton"),
                () =>
                {
                    FindObjectOfType<UIDocumentPlayerHUDV2>(true).gameObject.SetActive(true);

                    titleScreenManager.StartGame();

                    this.gameObject.SetActive(false);
                });
            menuManager.SetupButton(
                root.Q<Button>("ContinueButton"),
                () =>
                {
                    FindObjectOfType<UIDocumentTitleScreenLoadMenu>(true).gameObject.SetActive(true);
                    this.gameObject.SetActive(false);
                });
            menuManager.SetupButton(
                root.Q<Button>("CreditsButton"),
                () =>
                {
                    FindObjectOfType<UIDocumentTitleScreenCredits>(true).gameObject.SetActive(true);
                    this.gameObject.SetActive(false);
                });
            menuManager.SetupButton(
                root.Q<Button>("ControlsButton"),
                () =>
                {
                    FindObjectOfType<UIDocumentTitleScreenControls>(true).gameObject.SetActive(true);
                    this.gameObject.SetActive(false);
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

        private void Update()
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;

        }

    }
}