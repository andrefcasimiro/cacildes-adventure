using System.Collections.Generic;
using AF.Journals;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class Journal : MonoBehaviour
    {
        [Header("Book Settings")]
        [TextArea] public string title;
        public string author;

        public Transform journalPageContainer;
        public List<JournalPage> pages = new();
        public Color coverColor = new Color(58, 44, 33);

        [Header("Note Settings")]
        public bool isNote = false;

        [Header("Events")]
        public UnityEvent onRead_Begin;
        public UnityEvent onRead_End;

        [Header("Components")]
        public UIDocumentKeyPrompt documentKeyPrompt;
        public UIDocumentBook uIDocumentBook;
        public PlayerManager playerManager;

        private void Awake()
        {
            pages.Clear();

            foreach (Transform childTransform in journalPageContainer.transform)
            {
                childTransform.TryGetComponent(out JournalPage journalPage);

                if (journalPage != null)
                {
                    pages.Add(journalPage);
                }
            }
        }

        public void Read()
        {
            onRead_Begin?.Invoke();

            playerManager.playerComponentManager.DisableComponents();
            playerManager.playerComponentManager.DisableCharacterController();

            uIDocumentBook.BeginRead(this);
        }

        public void CloseBook()
        {
            uIDocumentBook.gameObject.SetActive(false);

            onRead_End?.Invoke();

            playerManager.playerComponentManager.EnableCharacterController();
            playerManager.playerComponentManager.EnableComponents();
        }
    }
}
