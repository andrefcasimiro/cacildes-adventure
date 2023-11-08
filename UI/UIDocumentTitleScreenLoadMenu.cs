using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{

    public class UIDocumentTitleScreenLoadMenu : MonoBehaviour
    {
        VisualElement root;
        public VisualTreeAsset loadButtonEntryPrefab;
        UIDocumentLoadMenu uIDocumentLoadMenu;
        UIDocumentTitleScreen uIDocumentTitleScreen;

        [Header("Components")]
        public UIManager uiManager;

        private void Awake()
        {
            uIDocumentLoadMenu = FindObjectOfType<UIDocumentLoadMenu>(true);
            uIDocumentTitleScreen = FindObjectOfType<UIDocumentTitleScreen>(true);

            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            root = GetComponent<UIDocument>().rootVisualElement;

            root.RegisterCallback<NavigationCancelEvent>(ev =>
            {
                Close();
            });

            DrawUI();
        }

        void DrawUI()
        {
            uIDocumentLoadMenu.DrawUI(root, true);

            uiManager.SetupButton(root.Q<Button>("CloseBtn"), () =>
            {
                Close();
            });

            uiManager.SetupButton(root.Q<Button>("ButtonOpenSaveFolder"), () =>
            {
                string path = System.IO.Path.Combine(Application.persistentDataPath);
                var itemPath = path.Replace(@"/", @"\");   // explorer doesn't like front slashes
                System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
            });
        }

        void Close()
        {
            if (uIDocumentTitleScreen != null)
            {
                uIDocumentTitleScreen.gameObject.SetActive(true);
            }

            gameObject.SetActive(false);
        }
    }
}
