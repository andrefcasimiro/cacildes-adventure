using AF.Journals;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentBook : MonoBehaviour
    {
        VisualElement root;

        Journal currentJournal;

        VisualElement bookFront, bookPage, bookBack, notePage, leftPage, rightPage;
        Label bookTitle, bookAuthor, notePageTitle, notePageText, leftPageTitle, leftPageText, rightPageTitle, rightPageText;

        [Header("Indexes")]
        public int currentPage = -1;

        [Header("Events")]
        public UnityEvent onJournalOpen;
        public UnityEvent onJournalClose;

        [Header("Components")]
        public Soundbank soundbank;
        public CursorManager cursorManager;

        private void Awake()
        {
            this.gameObject.SetActive(false);
        }

        void SetupRefs()
        {
            bookFront = this.root.Q<VisualElement>("BookFront");
            bookPage = this.root.Q<VisualElement>("BookPage");
            bookBack = this.root.Q<VisualElement>("BookBack");
            notePage = this.root.Q<VisualElement>("NotePage");

            bookTitle = this.root.Q<Label>("BookTitle");
            bookAuthor = this.root.Q<Label>("BookAuthor");

            notePageTitle = notePage.Q<Label>("ChapterTitle");
            notePageText = notePage.Q<Label>("PageText");

            leftPage = this.root.Q<VisualElement>("LeftPage");
            leftPageTitle = leftPage.Q<Label>("ChapterTitle");
            leftPageText = leftPage.Q<Label>("PageText");

            rightPage = this.root.Q<VisualElement>("RightPage");
            rightPageTitle = rightPage.Q<Label>("ChapterTitle");
            rightPageText = rightPage.Q<Label>("PageText");
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnClose()
        {
            if (currentJournal == null)
            {
                return;
            }

            onJournalClose.Invoke();
            currentJournal.CloseBook();
            currentJournal = null;

            cursorManager.HideCursor();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnSwitchNextPage()
        {
            if (currentJournal == null)
            {
                return;
            }

            NavigateBook(true);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnSwitchPreviousPage()
        {
            if (currentJournal == null)
            {
                return;
            }

            NavigateBook(false);
        }

        private void OnEnable()
        {
            this.root = GetComponent<UIDocument>().rootVisualElement;
            SetupRefs();
        }

        public void BeginRead(Journal journal)
        {
            this.currentJournal = journal;
            this.gameObject.SetActive(true);

            if (journal.isNote)
            {
                currentPage = 0;
                ShowNote();
            }
            else
            {
                currentPage = -1;
                ShowCover();
            }

            onJournalOpen?.Invoke();
        }

        public void ShowCover()
        {

            if (bookFront.style.display != DisplayStyle.Flex)
            {
                soundbank.PlaySound(soundbank.bookFlip);
            }

            bookFront.style.display = DisplayStyle.Flex;
            bookPage.style.display = DisplayStyle.None;
            bookBack.style.display = DisplayStyle.None;
            notePage.style.display = DisplayStyle.None;

            bookTitle.text = currentJournal.title;
            bookAuthor.text = currentJournal.author;

            bookFront.style.backgroundColor = currentJournal.coverColor;
        }

        public void ShowBack()
        {
            if (this.root.Q<VisualElement>("BookBack").style.display != DisplayStyle.Flex)
            {
                soundbank.PlaySound(soundbank.bookFlip);
            }

            bookFront.style.display = DisplayStyle.None;
            bookPage.style.display = DisplayStyle.None;
            bookBack.style.display = DisplayStyle.Flex;
            notePage.style.display = DisplayStyle.None;

            bookBack.style.backgroundColor = currentJournal.coverColor;
        }

        public void ShowNote()
        {
            soundbank.PlaySound(soundbank.bookFlip);

            bookFront.style.display = DisplayStyle.None;
            bookPage.style.display = DisplayStyle.None;
            bookBack.style.display = DisplayStyle.None;
            notePage.style.display = DisplayStyle.Flex;

            notePageTitle.text = currentJournal.pages[currentPage].pageTitle;
            notePageText.text = currentJournal.pages[currentPage].pageText;
        }

        public void ShowSinglePage(JournalPage page, bool renderOnRight)
        {
            soundbank.PlaySound(soundbank.bookFlip);

            bookFront.style.display = DisplayStyle.None;
            bookPage.style.display = DisplayStyle.Flex;
            bookBack.style.display = DisplayStyle.None;
            notePage.style.display = DisplayStyle.None;

            if (renderOnRight)
            {
                leftPageTitle.text = "";
                leftPageText.text = "";

                if (string.IsNullOrEmpty(page.pageTitle))
                {
                    rightPageTitle.style.display = DisplayStyle.None;
                }
                else
                {
                    rightPageTitle.text = page.pageTitle;
                    leftPageTitle.text = "";
                    rightPageTitle.style.display = DisplayStyle.Flex;
                }
                rightPageText.text = FormatText(page.pageText);
                leftPageText.text = "";
            }
            else
            {
                rightPageTitle.text = "";
                rightPageText.text = "";

                if (string.IsNullOrEmpty(page.pageTitle))
                {
                    leftPageTitle.style.display = DisplayStyle.None;
                }
                else
                {
                    leftPageTitle.text = page.pageTitle;
                    rightPageTitle.text = "";
                    leftPageTitle.style.display = DisplayStyle.Flex;
                }
                leftPageText.text = FormatText(page.pageText);
                rightPageText.text = "";
            }

        }

        public void ShowPages(JournalPage journalLeftPage, JournalPage journalRightPage)
        {
            soundbank.PlaySound(soundbank.bookFlip);

            bookFront.style.display = DisplayStyle.None;
            bookPage.style.display = DisplayStyle.Flex;
            bookBack.style.display = DisplayStyle.None;
            notePage.style.display = DisplayStyle.None;

            if (string.IsNullOrEmpty(journalLeftPage.pageTitle))
            {
                leftPageTitle.style.display = DisplayStyle.None;
            }
            else
            {
                leftPageTitle.text = journalLeftPage.pageTitle;
                leftPageTitle.style.display = DisplayStyle.Flex;
            }
            leftPageText.text = FormatText(journalLeftPage.pageText);


            if (string.IsNullOrEmpty(journalRightPage.pageTitle))
            {
                rightPageTitle.style.display = DisplayStyle.None;
            }
            else
            {
                rightPageTitle.text = journalRightPage.pageTitle;
                rightPageTitle.style.display = DisplayStyle.Flex;
            }
            rightPageText.text = FormatText(journalRightPage.pageText);
        }


        void NavigateBook(bool readForward)
        {
            if (currentJournal == null)
            {
                return;
            }

            if (currentJournal.isNote)
            {
                currentPage = 0;
                ShowNote();
                return;
            }

            if (readForward)
            {
                currentPage = Mathf.Clamp(
                    currentPage == -1 ? 0 : currentPage + 2,
                    -1,
                    currentJournal.pages.Count % 2 == 0 ? currentJournal.pages.Count : currentJournal.pages.Count + 1);
            }
            else
            {
                currentPage = Mathf.Clamp(
                    currentPage - 2,
                    -1,
                    currentJournal.pages.Count % 2 == 0 ? currentJournal.pages.Count : currentJournal.pages.Count + 1);
            }

            if (currentPage == -1)
            {
                ShowCover();
            }
            else if (currentPage > currentJournal.pages.Count - 1)
            {
                ShowBack();
            }
            else
            {
                if (currentPage + 1 > currentJournal.pages.Count - 1)
                {
                    ShowSinglePage(currentJournal.pages[currentPage], false);
                }
                else
                {
                    ShowPages(currentJournal.pages[currentPage], currentJournal.pages[currentPage + 1]);
                }
            }
        }

        string FormatText(string text)
        {
            return text.Replace("\r", "");
        }
    }
}
