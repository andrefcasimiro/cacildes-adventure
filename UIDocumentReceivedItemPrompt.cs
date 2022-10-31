using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace AF
{

    public class UIDocumentReceivedItemPrompt : MonoBehaviour
    {
        public UIDocument uiDocument;

        public AudioClip onConfirmSfx;

        VisualElement root;
        StarterAssetsInputs inputs;


        public string itemName = "Item Name";
        public int quantity = 0;
        public Sprite sprite = null;

        private void Awake()
        {
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            inputs = FindObjectOfType<StarterAssetsInputs>(true);

            this.root = uiDocument.rootVisualElement;


            if (Gamepad.current != null)
            {
                root.Q<IMGUIContainer>("KeyboardIcon").style.display = DisplayStyle.None;
                root.Q<IMGUIContainer>("GamepadIcon").style.display = DisplayStyle.Flex;
            }
            else
            {
                root.Q<IMGUIContainer>("KeyboardIcon").style.display = DisplayStyle.Flex;
                root.Q<IMGUIContainer>("GamepadIcon").style.display = DisplayStyle.None;
            }

            this.root.Q<VisualElement>("ActionButtons").Q<Label>("ItemName").text = itemName;
            this.root.Q<Label>("ItemQuantity").text = quantity.ToString();
            this.root.Q<IMGUIContainer>("ItemSprite").style.backgroundImage = new StyleBackground(sprite);

        }

        private void Update()
        {
            if (inputs.interact && this.gameObject.activeSelf)
            {
                inputs.interact = false;

                BGMManager.instance.PlaySound(onConfirmSfx, null);
                this.gameObject.SetActive(false);
            }
        }

    }

}
