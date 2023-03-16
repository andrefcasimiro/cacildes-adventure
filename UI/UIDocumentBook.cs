using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentBook : MonoBehaviour
    {
        VisualElement root;

        [HideInInspector] public ReadBook readBook;

        [Header("Localization")]
        public LocalizedText movePagesText;
        public LocalizedText arrowKeysText;
        public LocalizedText closeBookText;
        public LocalizedText tabText;

        StarterAssets.StarterAssetsInputs inputs => FindObjectOfType<StarterAssets.StarterAssetsInputs>(true);

        private void Awake()
        {
            this.gameObject.SetActive(false);
        }

        private void Update()
        {
            this.root.Focus();
        }

        void Translate(VisualElement root)
        {
            root.Q<Label>("MovePages").text = movePagesText.GetText();
            root.Q<Label>("ArrowKeys").text = arrowKeysText.GetText();
            root.Q<Label>("CloseBook").text = closeBookText.GetText();
            root.Q<Label>("Tab").text = tabText.GetText();
        }

        private void OnEnable()
        {
            this.root = GetComponent<UIDocument>().rootVisualElement;
            this.root.focusable = true;
            this.root.Focus();

            Translate(this.root);

            this.root.RegisterCallback<NavigationCancelEvent>(ev =>
            {
                GetReadBook().CloseBook();
            });

            this.root.RegisterCallback<NavigationMoveEvent>(ev =>
            {
                GetReadBook().NavigateBook(ev.direction == NavigationMoveEvent.Direction.Right);
            });

            this.root.RegisterCallback<KeyDownEvent>(ev =>
            {
                if (ev.keyCode == KeyCode.Tab || ev.keyCode == KeyCode.Escape || inputs.menu || Gamepad.current != null && Gamepad.current.buttonEast.isPressed)
                {
                    GetReadBook().CloseBook();
                }
                else if (ev.keyCode == KeyCode.RightArrow || Gamepad.current != null && Gamepad.current.leftStick.right.IsActuated())
                {
                    GetReadBook().NavigateBook(true);
                }
                else if (ev.keyCode == KeyCode.LeftArrow || Gamepad.current != null && Gamepad.current.leftStick.left.IsActuated())
                {
                    GetReadBook().NavigateBook(false);
                }
            });
        }

        ReadBook GetReadBook() { return this.readBook; }

        public void ShowCover(Book book)
        {
            if (this.root.Q<VisualElement>("BookFront").style.display != DisplayStyle.Flex)
            {
                Soundbank.instance.PlayBookFlip();
            }

            this.root.Q<VisualElement>("BookFront").style.display = DisplayStyle.Flex;
            this.root.Q<VisualElement>("BookPage").style.display = DisplayStyle.None;
            this.root.Q<VisualElement>("BookBack").style.display = DisplayStyle.None;
            this.root.Q<VisualElement>("NotePage").style.display = DisplayStyle.None;

            this.root.Q<Label>("BookTitle").text = book.bookTitle.GetText();
            this.root.Q<Label>("BookAuthor").text = book.bookAuthor.GetText();

            this.root.Q<VisualElement>("BookFront").style.backgroundColor = book.coverColor;
        }

        public void ShowBack(Book book)
        {
            if (this.root.Q<VisualElement>("BookBack").style.display != DisplayStyle.Flex)
            {
                Soundbank.instance.PlayBookFlip();
            }

            this.root.Q<VisualElement>("BookFront").style.display = DisplayStyle.None;
            this.root.Q<VisualElement>("BookPage").style.display = DisplayStyle.None;
            this.root.Q<VisualElement>("BookBack").style.display = DisplayStyle.Flex;
            this.root.Q<VisualElement>("NotePage").style.display = DisplayStyle.None;

            this.root.Q<VisualElement>("BookBack").style.backgroundColor = book.coverColor;
        }

        public void ShowNote(string noteTitle, string noteText)
        {
            Debug.Log("Show note");
            Soundbank.instance.PlayBookFlip();

            this.root.Q<VisualElement>("BookFront").style.display = DisplayStyle.None;
            this.root.Q<VisualElement>("BookPage").style.display = DisplayStyle.None;
            this.root.Q<VisualElement>("BookBack").style.display = DisplayStyle.None;
            this.root.Q<VisualElement>("NotePage").style.display = DisplayStyle.Flex;

            this.root.Q<VisualElement>("Page").Q<Label>("ChapterTitle").text = noteTitle;
            this.root.Q<VisualElement>("Page").Q<Label>("PageText").text = noteText;

        }

        public void ShowSinglePage(BookPage page, bool renderOnRight)
        {
            Soundbank.instance.PlayBookFlip();

            this.root.Q<VisualElement>("BookFront").style.display = DisplayStyle.None;
            this.root.Q<VisualElement>("BookPage").style.display = DisplayStyle.Flex;
            this.root.Q<VisualElement>("BookBack").style.display = DisplayStyle.None;
            this.root.Q<VisualElement>("NotePage").style.display = DisplayStyle.None;

            if (renderOnRight)
            {
                this.root.Q<VisualElement>("LeftPage").Q<Label>("ChapterTitle").text = "";
                this.root.Q<VisualElement>("LeftPage").Q<Label>("PageText").text = "";

                if (System.String.IsNullOrEmpty(page.chapterTitle.GetText()))
                {
                    this.root.Q<VisualElement>("RightPage").Q<Label>("ChapterTitle").style.display = DisplayStyle.None;
                }
                else
                {
                    this.root.Q<VisualElement>("RightPage").Q<Label>("ChapterTitle").text = page.chapterTitle.GetText();
                    this.root.Q<VisualElement>("LeftPage").Q<Label>("ChapterTitle").text = "";
                    this.root.Q<VisualElement>("RightPage").Q<Label>("ChapterTitle").style.display = DisplayStyle.Flex;
                }
                this.root.Q<VisualElement>("RightPage").Q<Label>("PageText").text = FormatText(page.pageText.GetText());
                this.root.Q<VisualElement>("LeftPage").Q<Label>("PageText").text = "";
            }
            else
            {
                this.root.Q<VisualElement>("RightPage").Q<Label>("ChapterTitle").text = "";
                this.root.Q<VisualElement>("RightPage").Q<Label>("PageText").text = "";

                if (System.String.IsNullOrEmpty(page.chapterTitle.GetText()))
                {
                    this.root.Q<VisualElement>("LeftPage").Q<Label>("ChapterTitle").style.display = DisplayStyle.None;
                }
                else
                {
                    this.root.Q<VisualElement>("LeftPage").Q<Label>("ChapterTitle").text = page.chapterTitle.GetText();
                    this.root.Q<VisualElement>("RightPage").Q<Label>("ChapterTitle").text = "";
                    this.root.Q<VisualElement>("LeftPage").Q<Label>("ChapterTitle").style.display = DisplayStyle.Flex;
                }
                this.root.Q<VisualElement>("LeftPage").Q<Label>("PageText").text = FormatText(page.pageText.GetText());
                this.root.Q<VisualElement>("RightPage").Q<Label>("PageText").text = "";
            }

        }

        public void ShowPages(BookPage leftPage, BookPage rightPage)
        {
            Soundbank.instance.PlayBookFlip();

            this.root.Q<VisualElement>("BookFront").style.display = DisplayStyle.None;
            this.root.Q<VisualElement>("BookPage").style.display = DisplayStyle.Flex;
            this.root.Q<VisualElement>("BookBack").style.display = DisplayStyle.None;
            this.root.Q<VisualElement>("NotePage").style.display = DisplayStyle.None;

            if (System.String.IsNullOrEmpty(leftPage.chapterTitle.GetText()))
            {
                this.root.Q<VisualElement>("LeftPage").Q<Label>("ChapterTitle").style.display = DisplayStyle.None;
            }
            else
            {
                this.root.Q<VisualElement>("LeftPage").Q<Label>("ChapterTitle").text = leftPage.chapterTitle.GetText();
                this.root.Q<VisualElement>("LeftPage").Q<Label>("ChapterTitle").style.display = DisplayStyle.Flex;
            }
            this.root.Q<VisualElement>("LeftPage").Q<Label>("PageText").text = FormatText(leftPage.pageText.GetText());


            if (System.String.IsNullOrEmpty(rightPage.chapterTitle.GetText()))
            {
                this.root.Q<VisualElement>("RightPage").Q<Label>("ChapterTitle").style.display = DisplayStyle.None;
            }
            else
            {
                this.root.Q<VisualElement>("RightPage").Q<Label>("ChapterTitle").text = rightPage.chapterTitle.GetText();
                this.root.Q<VisualElement>("RightPage").Q<Label>("ChapterTitle").style.display = DisplayStyle.Flex;
            }
            this.root.Q<VisualElement>("RightPage").Q<Label>("PageText").text = FormatText(rightPage.pageText.GetText());
        }

        string FormatText(string text)
        {
            return text.Replace("\r", "");
        }
    }
}
