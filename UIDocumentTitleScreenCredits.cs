using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{

    public class UIDocumentTitleScreenCredits : MonoBehaviour
    {
        VisualElement root;
        MenuManager menuManager;

        private void Awake()
        {
            menuManager = FindObjectOfType<MenuManager>(true);
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            this.root = GetComponent<UIDocument>().rootVisualElement;

            root.RegisterCallback<NavigationCancelEvent>(ev =>
            {
                Close();
            });

            DrawUI();
        }

        void DrawUI()
        {
            var closeBtn = root.Q<Button>("CloseBtn");


            var scrollContent = root.Q<ScrollView>().Children();
            foreach (var child in scrollContent)
            {
                child.RegisterCallback<FocusInEvent>(ev =>
                {
                    root.Q<ScrollView>().ScrollTo(child);
                });
            }


            menuManager.SetupButton(closeBtn, () =>
            {
                Close();
            });
        }

        void Close()
        {
            FindObjectOfType<UIDocumentTitleScreen>(true).gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }

        private void Update()
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }

    }

}
