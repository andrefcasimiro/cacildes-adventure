using UnityEngine;
using UnityEngine.EventSystems;

namespace AF
{
    public class MenuManager : InputListener
    {
        [Header("SFX")]
        public AudioClip selectSfx;
        public AudioClip decisionSfx;
        public AudioClip cancelSfx;

        public bool canOpenMenu = true;

        private void Start()
        {
            inputActions.PlayerActions.MainMenu.performed += ctx =>
            {
                if (canOpenMenu)
                {
                    EvaluateMenu();
                }
            };
        }

        private void EvaluateMenu()
        {
            if (IsInControlsScreen())
            {
                FindObjectOfType<UIDocumentControlsScreen>(true).Disable();
                FindObjectOfType<UIDocumentMainMenu>(true).Enable();
            }
            else if (IsInInventoryMenu())
            {
                FindObjectOfType<UIDocumentInventoryMenu>(true).Disable();
                FindObjectOfType<UIDocumentMainMenu>(true).Enable();
            }
            else if (IsInEquipmentSelectionMenu())
            {
                FindObjectOfType<UIDocumentEquipmentSelectionMenu>(true).Disable();
                FindObjectOfType<UIDocumentEquipmentMenu>(true).Enable();
            }
            else if (IsInEquipmentMenu())
            {
                FindObjectOfType<UIDocumentEquipmentMenu>(true).Disable();
                FindObjectOfType<UIDocumentMainMenu>(true).Enable();
            }
            else if (IsInMainMenu())
            {
                FindObjectOfType<UIDocumentMainMenu>(true).Disable();
            }
            else
            {
                PlayDecisionSfx();
                FindObjectOfType<UIDocumentMainMenu>(true).Enable();
            }

            UIDocumentPlayerHUD playerHUD = FindObjectOfType<UIDocumentPlayerHUD>(true);
            if (playerHUD != null)
            {
                if (IsMenuOpened())
                {
                    playerHUD.Disable();
                }
                else
                {
                    playerHUD.Enable();
                }
            }
        }

        public void DisableAllMenuWindows()
        {
            FindObjectOfType<UIDocumentMainMenu>(true).Disable();
        }

        public bool IsInControlsScreen()
        {
            UIDocumentControlsScreen uIDocumentControlsScreen = FindObjectOfType<UIDocumentControlsScreen>(true);

            if (uIDocumentControlsScreen.goBackToMenu == false)
            {
                return false;
            }

            if (uIDocumentControlsScreen == null)
            {
                return false;
            }

            return uIDocumentControlsScreen.IsVisible();
        }

        public bool IsInInventoryMenu()
        {
            UIDocumentInventoryMenu uIDocumentInventoryMenu = FindObjectOfType<UIDocumentInventoryMenu>(true);
            if (uIDocumentInventoryMenu == null)
            {
                return false;
            }

            return uIDocumentInventoryMenu.IsVisible();
        }

        public bool IsInEquipmentSelectionMenu()
        {
            UIDocumentEquipmentSelectionMenu uIDocumentEquipmentSelectionMenu = FindObjectOfType<UIDocumentEquipmentSelectionMenu>(true);
            if (uIDocumentEquipmentSelectionMenu == null)
            {
                return false;
            }

            return uIDocumentEquipmentSelectionMenu.IsVisible();
        }

        public bool IsInEquipmentMenu()
        {
            UIDocumentEquipmentMenu uIDocumentEquipmentMenu = FindObjectOfType<UIDocumentEquipmentMenu>(true);
            if (uIDocumentEquipmentMenu == null)
            {
                return false;
            }

            return uIDocumentEquipmentMenu.IsVisible();
        }

        public bool IsInMainMenu()
        {
            UIDocumentMainMenu uIDocumentMainMenu = FindObjectOfType<UIDocumentMainMenu>(true);
            if (uIDocumentMainMenu == null)
            {
                return false;
            }

            return uIDocumentMainMenu.IsVisible();
        }

        public bool IsMenuOpened()
        {

            return IsInInventoryMenu() || IsInMainMenu() || IsInEquipmentMenu() || IsInEquipmentSelectionMenu() || IsInControlsScreen();
        }

        public void PlaySelectSfx()
        {
            BGMManager.instance.PlaySound(selectSfx, null);
        }
        public void PlayDecisionSfx()
        {
            BGMManager.instance.PlaySound(decisionSfx, null);
        }
        public void PlayCancelSfx()
        {
            BGMManager.instance.PlaySound(cancelSfx, null);
        }

    }

}