using AF.Shops;
using AF.UI;
using UnityEngine;
using UnityEngine.Localization.Settings;

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


        [SerializeField] private UIDocumentBonfireMenu bonfireMenu;
        [SerializeField] private UIDocumentBonfireTravel bonfireTravel;
        [SerializeField] private UIDocumentLevelUp levelUp;
        [SerializeField] private UIDocumentShopMenu shopMenu;
        ViewMenu[] viewMenu;

        [SerializeField] private UIDocumentTitleScreen titleScreen;
        [SerializeField] private UIDocumentTitleScreenCredits screenCredits;
        [SerializeField] private UIDocumentTitleScreenOptions options;

        [Header("Components")]
        public NotificationManager notificationManager;

        private void Awake()
        {
            viewMenu = FindObjectsByType<ViewMenu>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        }

        public bool IsShowingGUI()
        {
            return CanShowGUI() == false;
        }

        public bool CanShowGUI()
        {
            if (IsShowingFullScreenGUI())
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

            if (screenCredits != null && screenCredits.isActiveAndEnabled)
            {
                return false;
            }

            if (options != null && options.isActiveAndEnabled)
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
            notificationManager.ShowNotification(LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Can not perform action at this time"), notificationManager.systemError);
        }

        public bool IsShowingFullScreenGUI()
        {
            // Check non-array UI elements
            if (craftScreen != null && craftScreen.isActiveAndEnabled)
            {
                return true;
            }

            if (book != null && book.isActiveAndEnabled)
            {
                return true;
            }

            if (dialogueWindow != null && dialogueWindow.isActiveAndEnabled)
            {
                return true;
            }

            if (gameOver != null && gameOver.isActiveAndEnabled)
            {
                return true;
            }
            if (bonfireMenu != null && bonfireMenu.isActiveAndEnabled)
            {
                return true;
            }

            if (bonfireTravel != null && bonfireTravel.isActiveAndEnabled)
            {
                return true;
            }

            if (levelUp != null && levelUp.isActiveAndEnabled)
            {
                return true;
            }

            if (shopMenu != null && shopMenu.isActiveAndEnabled)
            {
                return true;
            }

            return false;
        }
    }
}
