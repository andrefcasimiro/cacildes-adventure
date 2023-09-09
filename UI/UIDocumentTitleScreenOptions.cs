using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentTitleScreenOptions : MonoBehaviour
    {
        VisualElement root => GetComponent<UIDocument>().rootVisualElement;
        MenuManager menuManager;
        UIDocumentTitleScreen uIDocumentTitleScreen;

        [Header("Localization")]
        public LocalizedText optionsLabel;

        ViewComponent_GameSettings viewComponent_GameSettings => GetComponent<ViewComponent_GameSettings>();


        private void Awake()
        {
            menuManager = FindAnyObjectByType<MenuManager>(FindObjectsInactive.Include);
            uIDocumentTitleScreen = FindAnyObjectByType<UIDocumentTitleScreen>(FindObjectsInactive.Include);

            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            TranslateUI();

            root.RegisterCallback<NavigationCancelEvent>(ev =>
            {
                Close();
            });

            menuManager.SetupButton(root.Q<Button>("CloseBtn"), () =>
            {
                Close();
            });

            viewComponent_GameSettings.SetupRefs(root);
            viewComponent_GameSettings.TranslateSettingsUI(root);

            viewComponent_GameSettings.onLanguageChanged += () =>
            {
                TranslateUI();
            };
        }


        void TranslateUI()
        {
            root.Q<Label>("Title").text = optionsLabel.GetText();

        }
        void Close()
        {
            uIDocumentTitleScreen.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
