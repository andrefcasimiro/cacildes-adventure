using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentBonfireMenu : MonoBehaviour
    {
        public Bonfire bonfire;

        public UIDocumentLevelUp uiDocumentLevelUp;

        private void Start()
        {
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            root.Q<Label>("BonfireName").text = bonfire.bonfireName;

            root.Q<Button>("LeaveButton").RegisterCallback<ClickEvent>(ev =>
            {
                bonfire.ExitBonfire();
            });
            root.Q<Button>("LevelUpButton").RegisterCallback<ClickEvent>(ev =>
            {
                uiDocumentLevelUp.gameObject.SetActive(true);
                this.gameObject.SetActive(false);
            });

            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }


        private void Update()
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
    }

}
