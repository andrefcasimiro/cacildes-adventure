using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{

    public class UIDocumentControlsMenu : MonoBehaviour
    {
        MenuManager menuManager;
        VisualElement root;

        private void Awake()
        {
            menuManager = FindObjectOfType<MenuManager>(true);
        }

        private void OnEnable()
        {
            this.root = GetComponent<UIDocument>().rootVisualElement;
            menuManager.SetupNavMenu(this.root);
            menuManager.SetActiveMenu(this.root, "ButtonControls");

        }



    }

}