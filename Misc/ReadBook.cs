using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class ReadBook : MonoBehaviour
    {
        public Book book;

        [Header("Note")]
        public string noteTitle = "";

        [TextArea]
        public string[] notePages;

        public int page = -1;

        UIDocumentBook uIDocumentBook;

        PlayerComponentManager playerComponentManager;

        float maxNavigateCoolDown = 0.1f;
        float currentNavigateCooldown = Mathf.Infinity;

        [Header("Recipe Pickup")]
        public AlchemyRecipe alchemyRecipe;
        public CookingRecipe cookingRecipe;
        public string recipePickupSwitchUuid;
        Switch recipePickupSwitchInstance;

        [Header("Events")]
        public UnityEvent onRead;

        [Header("Switch")]
        public bool updateSwitchUponReading = false;
        public string switchUuid;
        public bool newSwitchValue;

        private void Awake()
        {
            playerComponentManager = FindObjectOfType<PlayerComponentManager>(true);
            uIDocumentBook = FindObjectOfType<UIDocumentBook>(true);
        }

        private void Start()
        {
            this.recipePickupSwitchInstance = SwitchManager.instance.GetSwitchInstance(this.recipePickupSwitchUuid);
        }

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
                    uIDocumentBook.ShowNote(page == 0 ? noteTitle : "", notePages[page]);
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
                SwitchManager.instance.UpdateSwitch(switchUuid, newSwitchValue);
            }

            if (recipePickupSwitchInstance != null && SwitchManager.instance.GetSwitchValue(recipePickupSwitchUuid) == false)
            {
                Soundbank.instance.PlayItemReceived();

                var notificationManager = FindObjectOfType<NotificationManager>(true);

                if (alchemyRecipe != null)
                {
                    notificationManager.ShowNotification("Learned recipe: " + alchemyRecipe.name, notificationManager.recipeIcon);
                    Player.instance.alchemyRecipes.Add(alchemyRecipe);
                }
                else if (cookingRecipe != null)
                {

                    notificationManager.ShowNotification("Learned recipe: " + cookingRecipe.name, notificationManager.recipeIcon);
                    Player.instance.cookingRecipes.Add(cookingRecipe);
                }

                SwitchManager.instance.UpdateSwitch(recipePickupSwitchUuid, true);
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
                uIDocumentBook.ShowNote(noteTitle, notePages[page]);
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

    }

}
