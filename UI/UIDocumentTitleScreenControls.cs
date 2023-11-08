using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    [RequireComponent(typeof(UIControlsLocalization))]
    public class UIDocumentTitleScreenControls : MonoBehaviour
    {
        VisualElement root;

        UIControlsLocalization uIControlsLocalization => GetComponent<UIControlsLocalization>();

        UIDocumentTitleScreen uIDocumentTitleScreen;

        [Header("Components")]
        public UIManager uiManager;

        private void Awake()
        {
            uIDocumentTitleScreen = FindObjectOfType<UIDocumentTitleScreen>(true);

            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            root = GetComponent<UIDocument>().rootVisualElement;

            uIControlsLocalization.Translate(root);

            root.RegisterCallback<NavigationCancelEvent>(ev =>
            {
                Close();
            });

            DrawUI();
        }

        void DrawUI()
        {
            var closeBtn = root.Q<Button>("CloseBtn");

            uiManager.SetupButton(closeBtn, () =>
            {
                Close();
            });
        }

        void Close()
        {
            uIDocumentTitleScreen.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
