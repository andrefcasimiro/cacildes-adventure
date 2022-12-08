using UnityEngine;
using UnityEngine.EventSystems;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Events;
using Mono.Cecil.Cil;
using UnityEngine.SceneManagement;

namespace AF
{
    public class MenuManager : MonoBehaviour
    {
        public GameObject equipmentListMenu;
        public GameObject equipmentSelectionMenu;
        public GameObject inventoryMenu;
        public GameObject saveMenu;
        public GameObject loadMenu;
        public GameObject controlsMenu;

        public StarterAssetsInputs starterAssetsInputs;

        public AudioClip clickSfx;
        public AudioClip hoverSfx;

        PlayerComponentManager playerComponentManager;

        [HideInInspector] public Texture2D screenshotBeforeOpeningMenu;

        UIDocumentAlchemyCraftScreen alchemyCraftScreen;

        ClimbController climbController;

        UIDocumentTitleScreen titleScreen;
        TitleScreenManager titleScreenManager;
        UIDocumentBook uIDocumentBook;
        UIDocumentGameOver uIDocumentGameOver;
        


        private void Start()
        {

            titleScreen = FindObjectOfType<UIDocumentTitleScreen>(true);
            titleScreenManager = FindObjectOfType<TitleScreenManager>(true);
            uIDocumentBook = FindObjectOfType<UIDocumentBook>(true);

            alchemyCraftScreen = FindObjectOfType<UIDocumentAlchemyCraftScreen>(true);

            playerComponentManager = FindObjectOfType<PlayerComponentManager>(true);

            climbController = playerComponentManager.GetComponent<ClimbController>();

            uIDocumentGameOver = FindObjectOfType<UIDocumentGameOver>(true);
        }

        public void CaptureScreenshot()
        {
            this.screenshotBeforeOpeningMenu = ScreenCapture.CaptureScreenshotAsTexture();
        }

        private void Update()
        {
            if (starterAssetsInputs.menu)
            {
                starterAssetsInputs.menu = false;
             
                if (!CanUseMenu())
                {
                    return;
                }
                
                PlayHover();
                this.screenshotBeforeOpeningMenu = ScreenCapture.CaptureScreenshotAsTexture();

                if (IsMenuOpen())
                {
                    this.CloseMenu();
                    starterAssetsInputs.cursorLocked = false;
                }
                else
                {
                    UnityEngine.Cursor.lockState = CursorLockMode.None;
                    this.OpenMenu();
                }
            }
        }

        bool CanUseMenu()
        {
            if (playerComponentManager.isInBonfire)
            {
                return false;
            }

            if (uIDocumentGameOver != null)
            {
                if (uIDocumentGameOver.isActiveAndEnabled)
                {
                    return false;
                }
            }

            if (titleScreenManager != null)
            {
                if (titleScreenManager.isActiveAndEnabled)
                {
                    return false;
                }
            }

            if (uIDocumentBook != null)
            {
                if (uIDocumentBook.isActiveAndEnabled)
                {
                    return false;
                }
            }

            if (alchemyCraftScreen.isActiveAndEnabled)
            {
                return false;
            }

            if (climbController.climbState != ClimbController.ClimbState.NONE)
            {
                return false;
            }

            return true;
        }

        public void OpenMenu()
        {
            equipmentListMenu.SetActive(true);
            equipmentSelectionMenu.SetActive(false);
            inventoryMenu.SetActive(false);
            saveMenu.SetActive(false);
            loadMenu.SetActive(false);
            controlsMenu.SetActive(false);
            playerComponentManager.DisableCharacterController();
            playerComponentManager.DisableComponents();

            FindObjectOfType<GamepadCursor>(true).gameObject.SetActive(true);
        }

        public void CloseMenu()
        {
            playerComponentManager.DisableComponents();

            if (equipmentSelectionMenu.activeSelf)
            {
                equipmentSelectionMenu.SetActive(false);
                equipmentListMenu.SetActive(true);

                return;
            }

            UnityEngine.Cursor.lockState = CursorLockMode.Locked;

            equipmentListMenu.SetActive(false);
            equipmentSelectionMenu.SetActive(false);
            inventoryMenu.SetActive(false);
            saveMenu.SetActive(false);
            loadMenu.SetActive(false);
            controlsMenu.SetActive(false);

            EventSystem.current.SetSelectedGameObject(null);
            playerComponentManager.EnableCharacterController();
            playerComponentManager.EnableComponents();

            FindObjectOfType<GamepadCursor>(true).gameObject.SetActive(false);
        }

        public bool IsMenuOpen()
        {
            return this.equipmentListMenu.activeSelf || this.equipmentSelectionMenu.activeSelf || this.inventoryMenu.activeSelf
                || this.saveMenu.activeSelf || this.loadMenu.activeSelf || this.controlsMenu.activeSelf;
        }

        public void OpenEquipmentListMenu()
        {
            this.inventoryMenu.SetActive(false);
            this.equipmentSelectionMenu.SetActive(false);
            this.equipmentListMenu.SetActive(true);
            this.saveMenu.SetActive(false);
            this.loadMenu.SetActive(false);
            this.controlsMenu.SetActive(false);
        }

        public void OpenEquipmentSelectionScreen(UIDocumentEquipmentSelectionMenuV2.EquipmentType equipmentType)
        {
            this.equipmentSelectionMenu.GetComponent<UIDocumentEquipmentSelectionMenuV2>().selectedEquipmentType = equipmentType;
            this.equipmentSelectionMenu.SetActive(true);
            this.equipmentListMenu.SetActive(false);
            this.inventoryMenu.SetActive(false);
            this.saveMenu.SetActive(false);
            this.loadMenu.SetActive(false);
            this.controlsMenu.SetActive(false);

        }

        public void OpenInventoryScreen()
        {
            this.inventoryMenu.SetActive(true);
            this.equipmentSelectionMenu.SetActive(false);
            this.equipmentListMenu.SetActive(false);
            this.saveMenu.SetActive(false);
            this.loadMenu.SetActive(false);
            this.controlsMenu.SetActive(false);

        }

        public void OpenSaveScreen()
        {
            this.inventoryMenu.SetActive(false);
            this.equipmentSelectionMenu.SetActive(false);
            this.equipmentListMenu.SetActive(false);
            this.saveMenu.SetActive(true);
            this.loadMenu.SetActive(false);
            this.controlsMenu.SetActive(false);

        }

        public void OpenLoadScreen()
        {
            this.inventoryMenu.SetActive(false);
            this.equipmentSelectionMenu.SetActive(false);
            this.equipmentListMenu.SetActive(false);
            this.saveMenu.SetActive(false);
            this.loadMenu.SetActive(true);
            this.controlsMenu.SetActive(false);

        }

        public void OpenControlsScreen()
        {
            this.inventoryMenu.SetActive(false);
            this.equipmentSelectionMenu.SetActive(false);
            this.equipmentListMenu.SetActive(false);
            this.saveMenu.SetActive(false);
            this.loadMenu.SetActive(false);
            this.controlsMenu.SetActive(true);

        }

        public void PlayClick()
        {
            BGMManager.instance.PlaySound(clickSfx, null);
        }

        public void PlayHover()
        {
            BGMManager.instance.PlaySound(hoverSfx, null);
        }

        public void SetActiveMenu(VisualElement root, string buttonNameToActivate)
        {
            root.Q<Button>("ButtonEquipment").RemoveFromClassList("active-menu-option");
            root.Q<Button>("ButtonInventory").RemoveFromClassList("active-menu-option");
            root.Q<Button>("ButtonSave").RemoveFromClassList("active-menu-option");
            root.Q<Button>("ButtonLoad").RemoveFromClassList("active-menu-option");
            root.Q<Button>("ButtonControls").RemoveFromClassList("active-menu-option");
            root.Q<Button>(buttonNameToActivate).AddToClassList("active-menu-option");
        }

        public void SetupNavMenu(VisualElement root)
        {
            root.RegisterCallback<NavigationCancelEvent>(ev =>
            {
                CloseMenu();
            });

            SetupButton(root.Q<Button>("ButtonEquipment"), () => { OpenEquipmentListMenu(); });
            SetupButton(root.Q<Button>("ButtonInventory"), () => { OpenInventoryScreen(); });
            SetupButton(root.Q<Button>("ButtonSave"), () => { OpenSaveScreen(); });
            SetupButton(root.Q<Button>("ButtonLoad"), () => { OpenLoadScreen(); });
            SetupButton(root.Q<Button>("ButtonControls"), () => { OpenControlsScreen(); });
            SetupButton(root.Q<Button>("ButtonExit"), () => {

                SaveSystem.instance.SaveGameData(SceneManager.GetActiveScene().name);

                Application.Quit();
            });
        }

        public void SetupButton(Button button, UnityAction callback)
        {
            button.RegisterCallback<ClickEvent>(ev => {
                BGMManager.instance.PlayUIDecision();
                callback.Invoke();
            });

            button.RegisterCallback<NavigationSubmitEvent>(ev => {
                BGMManager.instance.PlayUIDecision();
                callback.Invoke();
            });

            button.RegisterCallback<FocusEvent>(ev => {
                BGMManager.instance.PlayUISelect();
            });

            button.RegisterCallback<MouseOverEvent>(ev => {
                BGMManager.instance.PlayUISelect();
            });
        }
    }
}
