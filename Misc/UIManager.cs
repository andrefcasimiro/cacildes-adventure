using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace AF
{

    public class UIManager : MonoBehaviour
    {

        [Header("Critical UIs")]
        [SerializeField] private UIDocumentAlchemyCraftScreen alchemyCraftScreen;
        [SerializeField] private UIDocumentBlacksmithScreen blacksmithScreen;
        [SerializeField] private UIDocumentBook book;
        [SerializeField] private UIDocumentDialogueWindow dialogueWindow;
        [SerializeField] private UIDocumentGameOver gameOver;
        [SerializeField] private UIDocumentKeyPrompt keyPrompt;
        [SerializeField] private UIDocumentReceivedItemPrompt itemPrompt;
        [SerializeField] private ViewClockMenu viewClockMenu;


        UIDocumentBonfireMenu[] bonfireMenu;
        UIDocumentBonfireTravel[] bonfireTravel;
        UIDocumentLevelUp[] levelUp;
        UIDocumentShopMenu[] shopMenu;
        ViewMenu[] viewMenu;
        ReadableUIDocument[] readableUIDocuments;

        private UIDocumentTitleScreen titleScreen;
        private UIDocumentTitleScreenControls screenControls;
        private UIDocumentTitleScreenCredits screenCredits;
        private UIDocumentTitleScreenLoadMenu loadMenu;
        private UIDocumentTitleScreenOptions options;

        [Header("Components")]
        public NotificationManager notificationManager;

        private void Awake()
        {
            bonfireMenu = FindObjectsByType<UIDocumentBonfireMenu>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            bonfireTravel = FindObjectsByType<UIDocumentBonfireTravel>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            levelUp = FindObjectsByType<UIDocumentLevelUp>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            shopMenu = FindObjectsByType<UIDocumentShopMenu>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            viewMenu = FindObjectsByType<ViewMenu>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            readableUIDocuments = FindObjectsByType<ReadableUIDocument>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            titleScreen = FindAnyObjectByType<UIDocumentTitleScreen>(FindObjectsInactive.Include);
            screenControls = FindAnyObjectByType<UIDocumentTitleScreenControls>(FindObjectsInactive.Include);
            screenCredits = FindAnyObjectByType<UIDocumentTitleScreenCredits>(FindObjectsInactive.Include);
            loadMenu = FindAnyObjectByType<UIDocumentTitleScreenLoadMenu>(FindObjectsInactive.Include);
            options = FindAnyObjectByType<UIDocumentTitleScreenOptions>(FindObjectsInactive.Include);
        }

        public bool CanShowGUI()
        {
            // Check non-array UI elements
            if (alchemyCraftScreen != null && alchemyCraftScreen.isActiveAndEnabled)
            {
                return false;
            }

            if (blacksmithScreen != null && blacksmithScreen.isActiveAndEnabled)
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

            if (loadMenu != null && loadMenu.isActiveAndEnabled)
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

            // Check array UI elements
            foreach (var element in bonfireMenu)
            {
                if (element != null && element.isActiveAndEnabled)
                {
                    return false;
                }
            }

            foreach (var element in bonfireTravel)
            {
                if (element != null && element.isActiveAndEnabled)
                {
                    return false;
                }
            }

            foreach (var element in levelUp)
            {
                if (element != null && element.isActiveAndEnabled)
                {
                    return false;
                }
            }

            foreach (var element in shopMenu)
            {
                if (element != null && element.isActiveAndEnabled)
                {
                    return false;
                }
            }

            foreach (var element in viewMenu)
            {
                if (element != null && element.isActiveAndEnabled)
                {
                    return false;
                }
            }

            foreach (var element in readableUIDocuments)
            {
                if (element != null && element.document.isActiveAndEnabled)
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


        public void SetupButton(Button button, UnityAction callback)
        {
            button.RegisterCallback<ClickEvent>(ev =>
            {
                PlayPopAnimation(button);
                Soundbank.instance.PlayUIDecision();
                callback.Invoke();
            });

            button.RegisterCallback<NavigationSubmitEvent>(ev =>
            {
                PlayPopAnimation(button);
                Soundbank.instance.PlayUIDecision();
                callback.Invoke();
            });


            button.RegisterCallback<FocusEvent>(ev =>
            {
                PlayPopAnimation(button);
                Soundbank.instance.PlayUIHover();
            });

            button.RegisterCallback<MouseOverEvent>(ev =>
            {
                PlayPopAnimation(button);
                Soundbank.instance.PlayUIHover();
            });

            button.RegisterCallback<PointerEnterEvent>(ev =>
            {
                PlayPopAnimation(button);
                Soundbank.instance.PlayUIHover();
            });
        }

        void PlayPopAnimation(Button button)
        {
            DOTween.To(
                () => new Vector3(0, 0, 0),
                scale => button.transform.scale = scale,
                button.transform.scale,
                0.5f
            ).SetEase(Ease.OutElastic);

        }
    }
}
