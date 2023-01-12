using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{

    public class UIDocumentTitleScreenLoadMenu : MonoBehaviour
    {
        VisualElement root;
        public VisualTreeAsset loadButtonEntryPrefab;
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

            FindObjectOfType<UIDocumentLoadMenu>(true).DrawUI(this.root, true);


           
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
            FindObjectOfType<UIDocumentTitleScreen>(true).gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }

        private void Update()
        {
            // UnityEngine.Cursor.lockState = CursorLockMode.None;
        }

    }

}