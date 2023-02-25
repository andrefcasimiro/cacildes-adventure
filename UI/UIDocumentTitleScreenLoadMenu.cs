using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{

    public class UIDocumentTitleScreenLoadMenu : MonoBehaviour
    {
        VisualElement root;
        public VisualTreeAsset loadButtonEntryPrefab;
        MenuManager menuManager;

        UIDocumentLoadMenu uIDocumentLoadMenu => FindObjectOfType<UIDocumentLoadMenu>(true);
        UIDocumentTitleScreen uIDocumentTitleScreen => FindObjectOfType<UIDocumentTitleScreen>(true);

        private void Awake()
        {
            menuManager = FindObjectOfType<MenuManager>(true);

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

            menuManager.SetupButton(root.Q<Button>("CloseBtn"), () =>
            {
                Close();
            });

            menuManager.SetupButton(root.Q<Button>("ButtonOpenSaveFolder"), () =>
            {
                string path = System.IO.Path.Combine(Application.persistentDataPath);
                var itemPath = path.Replace(@"/", @"\");   // explorer doesn't like front slashes
                System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
            });
        }

        void Close()
        {
            uIDocumentTitleScreen.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

    }
}
