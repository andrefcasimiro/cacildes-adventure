using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using DG.Tweening;

namespace AF
{
    public class UIDocumentKeyPrompt : MonoBehaviour
    {
        public UIDocument uiDocument => GetComponent<UIDocument>();

        [Header("Components")]
        public Soundbank soundbank;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void DisplayPrompt(string key, string action)
        {
            this.gameObject.SetActive(true);

            VisualElement root = uiDocument.rootVisualElement;

            soundbank.PlaySound(soundbank.uiHover);

            DOTween.To(
                  () => root.contentContainer.style.opacity.value,
                  (value) => root.contentContainer.style.opacity = value,
                  1,
                  .25f
            );

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
    }
}
