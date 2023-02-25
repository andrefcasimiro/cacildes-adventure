using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentKeyPrompt : MonoBehaviour
    {
        public UIDocument uiDocument => GetComponent<UIDocument>();

        public string key = "E";
        public string action = "Pick Item";
        [HideInInspector] public string eventUuid = "";

        VisualElement root;

        private void Awake()
        {
            gameObject.SetActive(false);
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
