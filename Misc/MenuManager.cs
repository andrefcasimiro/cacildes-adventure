using AF.Ladders;
using AF.UI.EquipmentMenu;
using UnityEngine;

namespace AF
{
    public enum ActiveMenu
    {
        EQUIPMENT,
        OPTIONS,
        OBJECTIVES,
        EXIT_GAME,
    }

    public class MenuManager : MonoBehaviour
    {
        public ActiveMenu activeMenu = ActiveMenu.EQUIPMENT;

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
        public ViewEquipmentMenu viewEquipmentMenu;
        public ViewSettingsMenu viewSettingsMenu;
        public ViewObjectivesMenu viewObjectivesMenu;

        public PlayerManager playerManager;

        private void Start()
        {
            starterAssetsInputs.onMenuInput += EvaluateMenu;
        }

        void EvaluateMenu()
        {
            starterAssetsInputs.menu = false;

            if (!CanUseMenu())
            {
                return;
            }

            hasPlayedFadeIn = false;
            if (IsMenuOpen())
            {
                CloseMenu();
                cursorManager.HideCursor();
            }
            else
            {
                OpenMenu();
                cursorManager.ShowCursor();
            }
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

        public void SetMenuView(ActiveMenu activeMenu)
        {
            this.activeMenu = activeMenu;

            CloseMenuViews();

            if (activeMenu == ActiveMenu.EQUIPMENT)
            {
                viewEquipmentMenu.gameObject.SetActive(true);
            }
            if (activeMenu == ActiveMenu.OPTIONS)
            {
                viewSettingsMenu.gameObject.SetActive(true);
            }
            if (activeMenu == ActiveMenu.OBJECTIVES)
            {
                viewObjectivesMenu.gameObject.SetActive(true);
            }
        }

        public void OpenMenu()
        {
            cursorManager.ShowCursor();

            playerManager.thirdPersonController.LockCameraPosition = true;

            SetMenuView(activeMenu);
        }

        void CloseMenuViews()
        {
            viewEquipmentMenu.gameObject.SetActive(false);
            viewSettingsMenu.gameObject.SetActive(false);
            viewObjectivesMenu.gameObject.SetActive(false);
        }

        public void CloseMenu()
        {
            playerManager.thirdPersonController.LockCameraPosition = false;
            CloseMenuViews();

            return;
        }

        public bool IsMenuOpen()
        {
            if (viewEquipmentMenu.isActiveAndEnabled)
            {
                return true;
            }

            if (viewSettingsMenu.isActiveAndEnabled)
            {
                return true;
            }

            if (viewObjectivesMenu.isActiveAndEnabled)
            {
                return true;
            }

            return false;
        }
    }
}
