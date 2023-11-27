using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    [System.Serializable]
    public class BookPage
    {
        public LocalizedText chapterTitle;

        public LocalizedText pageText;
    }

    [CreateAssetMenu(menuName = "Misc / Books / New Book")]
    public class Book : ScriptableObject
    {
        public LocalizedText bookTitle;
        public LocalizedText bookAuthor;

        public BookPage[] bookPages;

        public Color coverColor = new Color(58, 44, 33);

    }

}
