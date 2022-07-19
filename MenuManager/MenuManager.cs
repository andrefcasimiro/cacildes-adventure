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

        SFXManager sfxManager;

        private void Awake()
        {
            sfxManager = FindObjectOfType<SFXManager>(true);
        }

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
            if (IsInInventoryMenu())
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

            return IsInInventoryMenu() || IsInMainMenu() || IsInEquipmentMenu() || IsInEquipmentSelectionMenu();
        }

        public void PlaySelectSfx()
        {
            sfxManager.PlaySound(selectSfx, null);
        }
        public void PlayDecisionSfx()
        {
            sfxManager.PlaySound(decisionSfx, null);
        }
        public void PlayCancelSfx()
        {
            sfxManager.PlaySound(cancelSfx, null);
        }

    }

}