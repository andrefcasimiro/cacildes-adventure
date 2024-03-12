using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentTitleScreenOptions : MonoBehaviour
    {
        VisualElement root => GetComponent<UIDocument>().rootVisualElement;
        UIDocumentTitleScreen uIDocumentTitleScreen;

        ViewComponent_GameSettings viewComponent_GameSettings => GetComponent<ViewComponent_GameSettings>();

        [Header("Components")]
        public UIManager uiManager;
        public Soundbank soundbank;

        private void Awake()
        {
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

            UIUtils.SetupButton(root.Q<Button>("CloseBtn"), () =>
            {
                Close();
            }, soundbank);

            viewComponent_GameSettings.SetupRefs(root);

            viewComponent_GameSettings.onLanguageChanged += () =>
            {
                TranslateUI();
            };
        }


        void TranslateUI()
        {
            root.Q<Label>("Title").text = "Options";
        }

        void Close()
        {
            uIDocumentTitleScreen.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
