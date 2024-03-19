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

        // Scene Refs
        UIDocumentBook uIDocumentBook;
        PlayerManager playerManager;

        UIDocumentBook GetUIDocumentBook()
        {
            if (uIDocumentBook == null) { uIDocumentBook = FindAnyObjectByType<UIDocumentBook>(FindObjectsInactive.Include); }
            return uIDocumentBook;
        }

        PlayerManager GetPlayerManager()
        {
            if (playerManager == null) { playerManager = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include); }
            return playerManager;
        }

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

            GetPlayerManager().playerComponentManager.DisableComponents();
            GetPlayerManager().playerComponentManager.DisableCharacterController();

            GetUIDocumentBook().BeginRead(this);
        }

        public void CloseBook()
        {
            GetUIDocumentBook().gameObject.SetActive(false);

            onRead_End?.Invoke();

            GetPlayerManager().playerComponentManager.EnableCharacterController();
            GetPlayerManager().playerComponentManager.EnableComponents();
        }
    }
}
