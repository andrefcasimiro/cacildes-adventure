using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace AF
{
    public class ReadableUIDocument : MonoBehaviour, IEventNavigatorCapturable
    {

        float maxNavigateCoolDown = 0.1f;
        float currentNavigateCooldown = Mathf.Infinity;

        public UIDocument document;

        [Header("Translation")]
        public bool useTranslation = false;
        public VisualTreeAsset portugueseDocument;
        public VisualTreeAsset englishDocument;

        [Header("Components")]
        public UIDocumentKeyPrompt documentKeyPrompt;
        public PlayerComponentManager playerComponentManager;
        public StarterAssetsInputs inputs;
        public MenuManager menuManager;

        VisualElement root;

        private void Awake()
        {
            document.enabled = false;
        }

        private void Start()
        {
        }

        public void Read()
        {
            menuManager.canUseMenu = false;
            playerComponentManager.DisableCharacterController();
            playerComponentManager.DisableComponents();


            document.enabled = true;

            Soundbank.instance.PlayUIEquip();

            if (this.root == null)
            {
                this.root = document.rootVisualElement;
            }


            if (useTranslation)
            {
                if (GamePreferences.instance.IsEnglish())
                {
                    document.rootVisualElement.Clear();
                    document.rootVisualElement.Add(englishDocument.CloneTree());
                }
                else
                {
                    document.rootVisualElement.Clear();
                    document.rootVisualElement.Add(portugueseDocument.CloneTree());
                }
            }
        }

        private void Update()
        {
            if (document.isActiveAndEnabled)
            {
                if (Gamepad.current != null && Gamepad.current.buttonEast.isPressed)
                {
                    CloseBook();
                }
            }
        }

        public void CloseBook()
        {
            playerComponentManager.EnableCharacterController();
            playerComponentManager.EnableComponents();

            this.root = null;
            document.enabled = false;
            menuManager.canUseMenu = true;
        }

        public bool IsReading()
        {
            return document.enabled;
        }

        public void OnCaptured()
        {
            if (IsReading())
            {
                return;
            }

            documentKeyPrompt.DisplayPrompt("E", LocalizedTerms.Read());
        }

        public void OnInvoked()
        {
            if (document.enabled)
            {
                return;
            }

            Read();
            documentKeyPrompt.gameObject.SetActive(false);
        }

        public void OnReleased()
        {
            throw new System.NotImplementedException();
        }
    }
}
