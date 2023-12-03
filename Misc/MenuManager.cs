using System;
using AF.Ladders;
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

        public PlayerManager playerManager;

        [Header("Events")]
        public UnityEvent onMenuOpen;
        public UnityEvent onMenuClose;

        [Header("Flags")]
        public bool isMenuOpen;

        [Header("Menu Views")]
        public ViewMenu[] viewMenus;
        public int viewMenuIndex = 0;

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

            if (isMenuOpen)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
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
            cursorManager.ShowCursor();

            viewMenuIndex = 0;
            SetMenuView();

            playerManager.thirdPersonController.LockCameraPosition = true;
            onMenuOpen?.Invoke();
        }

        public void CloseMenu()
        {
            isMenuOpen = false;

            cursorManager.HideCursor();

            CloseMenuViews();

            playerManager.thirdPersonController.LockCameraPosition = false;
            onMenuClose?.Invoke();
        }

        void CloseMenuViews()
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
            viewMenuIndex++;
            SetMenuView();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnPreviousMenu()
        {
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

            if (playerManager.climbController.climbState != ClimbState.NONE)
            {
                return false;
            }

            return true;
        }
    }
}
