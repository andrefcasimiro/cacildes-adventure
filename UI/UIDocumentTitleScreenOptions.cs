using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentTitleScreenOptions : MonoBehaviour
    {
        public UIDocumentOptionsMenu uIDocumentOptionsMenu;
        VisualElement root => GetComponent<UIDocument>().rootVisualElement;
        MenuManager menuManager => FindObjectOfType<MenuManager>(true);
        UIDocumentTitleScreen uIDocumentTitleScreen => FindObjectOfType<UIDocumentTitleScreen>(true);

        [Header("Localization")]
        public LocalizedText optionsLabel;

        private void Awake()
        {
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

            uIDocumentOptionsMenu.Activate(root);
            uIDocumentOptionsMenu.onLanguageChanged += () =>
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
