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
        public PlayerComponentManager playerComponentManager;
        public UIDocumentAlchemyCraftScreen alchemyCraftScreen;
        public ClimbController climbController;
        public TitleScreenManager titleScreenManager;
        public UIDocumentBook uIDocumentBook;
        public UIDocumentGameOver uIDocumentGameOver;
        public ThirdPersonController thirdPersonController;
        public ViewEquipmentMenu viewEquipmentMenu;
        public ViewSettingsMenu viewSettingsMenu;
        public ViewObjectivesMenu viewObjectivesMenu;

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

            if (playerComponentManager.IsBusy())
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

            if (FindFirstObjectByType<UIDocumentShopMenu>())
            {
                return false;
            }

            if (FindFirstObjectByType<UIDocumentBlacksmithScreen>())
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

            thirdPersonController.LockCameraPosition = true;

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
            thirdPersonController.LockCameraPosition = false;
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
