using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    [RequireComponent(typeof(UIControlsLocalization))]
    public class UIDocumentControlsMenu : MonoBehaviour
    {
        MenuManager menuManager;
        VisualElement root;

        UIControlsLocalization uIControlsLocalization => GetComponent<UIControlsLocalization>();

        private void Awake()
        {
            menuManager = FindObjectOfType<MenuManager>(true);
        }

        private void OnEnable()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            uIControlsLocalization.Translate(root);

            menuManager.SetupNavMenu(root);
            menuManager.SetActiveMenu(root, "ButtonControls");
        }
    }
}
