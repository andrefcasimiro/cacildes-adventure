using System;
using System.Collections;
using AF.Ladders;
using AF.Shops;
using AF.UI;
using AF.UI.EquipmentMenu;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class MenuManager : MonoBehaviour
    {

        public bool canUseMenu = true;

        [Header("Visuals")]
        [HideInInspector] public bool hasPlayedFadeIn = false;

        [Header("Components")]

        public CursorManager cursorManager;
        public StarterAssetsInputs starterAssetsInputs;
        public UIDocumentCraftScreen craftScreen;
        public TitleScreenManager titleScreenManager;
        public UIDocumentBook uIDocumentBook;
        public UIDocumentGameOver uIDocumentGameOver;
        public UIDocumentShopMenu uIDocumentShopMenu;
        public UIDocumentCharacterCustomization uIDocumentCharacterCustomization;

        public PlayerManager playerManager;

        [Header("Events")]
        public UnityEvent onMenuOpen;
        public UnityEvent onMenuClose;

        [Header("Flags")]
        public bool isMenuOpen;

        [Header("Menu Views")]
        public ViewMenu[] viewMenus;
        public int viewMenuIndex = 0;

        [Header("Nested Edge Case")]
        public ItemList itemList;


        public Texture2D screenshotBeforeOpeningMenu;


        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnMenuInput()
        {
            if (!CanUseMenu())
            {
                return;
            }

            hasPlayedFadeIn = false;

            if (itemList.isActiveAndEnabled)
            {
                itemList.ReturnToEquipmentSlots();
            }
            else if (!isMenuOpen)
            {
                StartCoroutine(CaptureScreenshot());

                OpenMenu();
            }
            else
            {
                CloseMenu();
            }
        }

        IEnumerator CaptureScreenshot()
        {
            // Wait until the end of the frame
            yield return new WaitForEndOfFrame();

            // Capture the screenshot as a texture
            Texture2D screenshotTexture = ScreenCapture.CaptureScreenshotAsTexture();
            if (screenshotTexture != null)
            {
                this.screenshotBeforeOpeningMenu = screenshotTexture;
            }
        }

        /// <summary>
        /// UnityEvent
        /// </summary>
        public void OnCancelInput()
        {
            if (!CanUseMenu() || isMenuOpen == false)
            {
                return;
            }

            hasPlayedFadeIn = false;

            if (itemList.isActiveAndEnabled)
            {
                itemList.ReturnToEquipmentSlots();
            }
            else if (isMenuOpen)
            {
                CloseMenu();
            }
        }

        public void SetMenuView()
        {
            CloseMenuViews();

            if (viewMenuIndex >= viewMenus.Length)
            {
                viewMenuIndex = 0;
            }
            else if (viewMenuIndex < 0)
            {
                viewMenuIndex = viewMenus.Length - 1;
            }

            viewMenus[viewMenuIndex].gameObject.SetActive(true);
        }

        public void OpenMenu()
        {
            isMenuOpen = true;

            viewMenuIndex = 0;
            SetMenuView();

            playerManager.playerComponentManager.DisablePlayerControl();
            playerManager.thirdPersonController.LockCameraPosition = true;

            onMenuOpen?.Invoke();
            cursorManager.ShowCursor();
        }

        public void CloseMenu()
        {
            isMenuOpen = false;

            CloseMenuViews();

            playerManager.thirdPersonController.LockCameraPosition = false;

            playerManager.playerComponentManager.EnablePlayerControl();

            onMenuClose?.Invoke();

            cursorManager.HideCursor();
        }

        public void CloseMenuViews()
        {
            foreach (ViewMenu viewMenu in viewMenus)
            {
                viewMenu.gameObject.SetActive(false);
            }
        }


        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnNextMenu()
        {
            if (!isMenuOpen)
            {
                return;
            }

            viewMenuIndex++;
            SetMenuView();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnPreviousMenu()
        {
            if (!isMenuOpen)
            {
                return;
            }

            viewMenuIndex--;
            SetMenuView();
        }

        bool CanUseMenu()
        {
            if (!canUseMenu)
            {
                return false;
            }

            if (playerManager.IsBusy())
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

            if (craftScreen.isActiveAndEnabled)
            {
                return false;
            }

            if (uIDocumentShopMenu.isActiveAndEnabled)
            {
                return false;
            }

            if (playerManager.climbController.climbState != ClimbState.NONE)
            {
                return false;
            }

            if (uIDocumentCharacterCustomization.isActiveAndEnabled)
            {
                return false;
            }

            return true;
        }
    }
}
