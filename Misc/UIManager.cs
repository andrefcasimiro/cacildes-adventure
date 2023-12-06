using AF.Shops;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace AF
{

    public class UIManager : MonoBehaviour
    {

        [Header("Critical UIs")]
        [SerializeField] private UIDocumentCraftScreen craftScreen;
        [SerializeField] private UIDocumentBook book;
        [SerializeField] private UIDocumentDialogueWindow dialogueWindow;
        [SerializeField] private UIDocumentGameOver gameOver;
        [SerializeField] private UIDocumentKeyPrompt keyPrompt;
        [SerializeField] private UIDocumentReceivedItemPrompt itemPrompt;
        [SerializeField] private ViewClockMenu viewClockMenu;


        [SerializeField] private UIDocumentBonfireMenu bonfireMenu;
        [SerializeField] private UIDocumentBonfireTravel bonfireTravel;
        [SerializeField] private UIDocumentLevelUp levelUp;
        [SerializeField] private UIDocumentShopMenu shopMenu;
        ViewMenu[] viewMenu;

        [SerializeField] private UIDocumentTitleScreen titleScreen;
        [SerializeField] private UIDocumentTitleScreenControls screenControls;
        [SerializeField] private UIDocumentTitleScreenCredits screenCredits;
        [SerializeField] private UIDocumentTitleScreenOptions options;

        [Header("Components")]
        public NotificationManager notificationManager;

        private void Awake()
        {
            viewMenu = FindObjectsByType<ViewMenu>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        }

        public bool CanShowGUI()
        {
            // Check non-array UI elements
            if (craftScreen != null && craftScreen.isActiveAndEnabled)
            {
                return false;
            }

            if (book != null && book.isActiveAndEnabled)
            {
                return false;
            }

            if (dialogueWindow != null && dialogueWindow.isActiveAndEnabled)
            {
                return false;
            }

            if (gameOver != null && gameOver.isActiveAndEnabled)
            {
                return false;
            }

            if (itemPrompt != null && itemPrompt.isActiveAndEnabled)
            {
                return false;
            }

            if (titleScreen != null && titleScreen.isActiveAndEnabled)
            {
                return false;
            }

            if (screenControls != null && screenControls.isActiveAndEnabled)
            {
                return false;
            }

            if (screenCredits != null && screenCredits.isActiveAndEnabled)
            {
                return false;
            }

            if (options != null && options.isActiveAndEnabled)
            {
                return false;
            }

            if (viewClockMenu != null && viewClockMenu.isActiveAndEnabled)
            {
                return false;
            }


            if (bonfireMenu != null && bonfireMenu.isActiveAndEnabled)
            {
                return false;
            }

            if (bonfireTravel != null && bonfireTravel.isActiveAndEnabled)
            {
                return false;
            }

            if (levelUp != null && levelUp.isActiveAndEnabled)
            {
                return false;
            }

            if (shopMenu != null && shopMenu.isActiveAndEnabled)
            {
                return false;
            }

            foreach (var element in viewMenu)
            {
                if (element != null && element.isActiveAndEnabled)
                {
                    return false;
                }
            }

            // If none of the elements are active and enabled, return true
            return true;
        }

        public void ShowCanNotAccessGUIAtThisTime()
        {
            notificationManager.ShowNotification("Can not perform action at this time", notificationManager.systemError);
        }

    }
}
