using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class ReadBook : MonoBehaviour, IEventNavigatorCapturable
    {
        public Book book;

        [Header("Note")]
        public LocalizedText noteTitle;

        public LocalizedText[] notePages;

        public int page = -1;

        [Header("Recipe Pickup")]
        public AlchemyRecipe alchemyRecipe;
        public CookingRecipe cookingRecipe;

        [Header("Events")]
        public UnityEvent onRead;

        [Header("Switch")]
        public bool updateSwitchUponReading = false;
        public SwitchEntry switchEntry;
        public bool newSwitchValue;

        float maxNavigateCoolDown = 0.1f;
        float currentNavigateCooldown = Mathf.Infinity;

        UIDocumentKeyPrompt documentKeyPrompt => FindObjectOfType<UIDocumentKeyPrompt>(true);
        UIDocumentBook uIDocumentBook => FindObjectOfType<UIDocumentBook>(true);
        PlayerComponentManager playerComponentManager => FindObjectOfType<PlayerComponentManager>(true);

        private void Update()
        {
            if (currentNavigateCooldown < maxNavigateCoolDown)
            {
                currentNavigateCooldown += Time.deltaTime;
            }
        }

        public void NavigateBook(bool readForward)
        {
            if (currentNavigateCooldown < maxNavigateCoolDown)
            {
                return;
            }

            currentNavigateCooldown = 0f;

            if (IsNote())
            {
                if (readForward)
                {
                    page = Mathf.Clamp(page == -1 ? 0 : page + 2, 0, notePages.Length % 2 == 0 ? notePages.Length : notePages.Length + 1);
                }
                else
                {

                    page = Mathf.Clamp(page - 2, 0, notePages.Length % 2 == 0 ? notePages.Length : notePages.Length + 1);
                }

                if (notePages[page] != null)
                {
                    uIDocumentBook.ShowNote(page == 0 ? noteTitle.GetText() : "", notePages[page].GetText());
                }

                return;
            }


            if (readForward)
            {
                page = Mathf.Clamp(page == -1 ? 0 : page + 2, -1, book.bookPages.Length % 2 == 0 ? book.bookPages.Length : book.bookPages.Length + 1);
            }
            else
            {
                
                page = Mathf.Clamp(page - 2, -1, book.bookPages.Length % 2 == 0 ? book.bookPages.Length  : book.bookPages.Length + 1);
            }


            if (page == -1)
            {
                uIDocumentBook.ShowCover(book);
            }
            else if (page > book.bookPages.Length - 1)
            {
                uIDocumentBook.ShowBack(book);
            }
            else {
                if (page + 1 > book.bookPages.Length - 1)
                {
                    uIDocumentBook.ShowSinglePage(book.bookPages[page], false);
                }
                else
                {
                    uIDocumentBook.ShowPages(book.bookPages[page], book.bookPages[page + 1]);
                }
            }
        }

        public void Read()
        {
            onRead.Invoke();

            if (updateSwitchUponReading)
            {
                if (SwitchManager.instance.GetSwitchCurrentValue(switchEntry) == false)
                {
                    if (alchemyRecipe != null || cookingRecipe != null)
                    {
                        Soundbank.instance.PlayItemReceived();

                        var notificationManager = FindObjectOfType<NotificationManager>(true);

                        if (alchemyRecipe != null)
                        {
                            notificationManager.ShowNotification(LocalizedTerms.LearnedRecipe() + alchemyRecipe.name, notificationManager.recipeIcon);
                            Player.instance.alchemyRecipes.Add(alchemyRecipe);
                        }
                        else if (cookingRecipe != null)
                        {
                            notificationManager.ShowNotification(LocalizedTerms.LearnedRecipe() + cookingRecipe.name, notificationManager.recipeIcon);
                            Player.instance.cookingRecipes.Add(cookingRecipe);
                        }
                    }

                    SwitchManager.instance.UpdateSwitch(switchEntry, true);
                }
            }

            if (IsNote())
            {
                page = 0;
            }
            else
            {
                page = -1;
            }

            playerComponentManager.DisableCharacterController();
            playerComponentManager.DisableComponents();

            uIDocumentBook.readBook = this;
            uIDocumentBook.gameObject.SetActive(true);

            if (!IsNote())
            {
                uIDocumentBook.ShowCover(book);
            }
            else
            {
                uIDocumentBook.ShowNote(noteTitle.GetText(), notePages[page].GetText());
            }
        }

        public void CloseBook()
        {
            playerComponentManager.EnableCharacterController();
            playerComponentManager.EnableComponents();
            uIDocumentBook.readBook = null;

            uIDocumentBook.gameObject.SetActive(false);
        }
        
        public bool IsReading()
        {
            return uIDocumentBook.readBook != null;
        }

        public bool IsNote()
        {
            return book == null;
        }

        public void OnCaptured()
        {
            if (IsReading())
            {
                return;
            }

            documentKeyPrompt.key = "E";

            string title;
            if (IsNote())
            {
                title = noteTitle.GetText();
            }
            else
            {
                title = book.name;
            }

            documentKeyPrompt.action = LocalizedTerms.Read() + " '" + title + "'";
            documentKeyPrompt.gameObject.SetActive(true);
        }

        public void OnInvoked()
        {
            Read();
            documentKeyPrompt.gameObject.SetActive(false);
        }
    }
}
