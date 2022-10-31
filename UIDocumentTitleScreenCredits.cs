using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{

    public class UIDocumentTitleScreenCredits : MonoBehaviour
    {
        VisualElement root;

        private void Awake()
        {
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            this.root = GetComponent<UIDocument>().rootVisualElement;
            DrawUI();
        }

        void DrawUI()
        {
            root.Q<Button>("CloseBtn").RegisterCallback<ClickEvent>(ev =>
            {
                FindObjectOfType<UIDocumentTitleScreen>(true).gameObject.SetActive(true);
                this.gameObject.SetActive(false);
            });
        }

        private void Update()
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }

    }

}
