using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace AF
{

    public class UIDocumentKeyPromptBase : MonoBehaviour
    {
        public UIDocument uiDocument;

        public string key = "E";
        public string action = "Pick Item";

        VisualElement root;

        private void Awake()
        {
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            root = uiDocument.rootVisualElement;

            if (Gamepad.current != null)
            {
                root.Q<IMGUIContainer>("KeyboardIcon").style.display = DisplayStyle.None;
                root.Q<IMGUIContainer>("GamepadIcon").style.display = DisplayStyle.Flex;
            }
            else
            {
                root.Q<IMGUIContainer>("KeyboardIcon").style.display = DisplayStyle.Flex;
                root.Q<IMGUIContainer>("GamepadIcon").style.display = DisplayStyle.None;

                root.Q<IMGUIContainer>("KeyboardIcon").Q<Label>("KeyText").text = key;
            }

            root.Q<Label>("InputText").text = action;
        }

        private void OnDisable()
        {
            action = "";
        }

    }

}