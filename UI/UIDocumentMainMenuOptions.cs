using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentMainMenuOptions : MonoBehaviour
    {
        public UIDocumentOptionsMenu uIDocumentOptionsMenu;
        VisualElement root => GetComponent<UIDocument>().rootVisualElement;
        MenuManager menuManager;

        private void Awake()
        {
             menuManager = FindObjectOfType<MenuManager>(true);

            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            menuManager.SetupNavMenu(this.root);
            menuManager.SetActiveMenu(this.root, "ButtonOptions");
            menuManager.TranslateNavbar(root);

            uIDocumentOptionsMenu.Activate(root);

            uIDocumentOptionsMenu.onLanguageChanged += () =>
            {
                menuManager.TranslateNavbar(this.root);
            };
        }
    }
}