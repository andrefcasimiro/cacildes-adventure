using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    [System.Serializable]
    public class BookPage
    {
        public string chapterTitle = "";

        [TextArea]
        public string pageText = "";
    }

    [CreateAssetMenu(menuName = "Misc / Books / New Book")]
    public class Book : ScriptableObject
    {
        public string bookTitle;
        public string bookAuthor;

        public BookPage[] bookPages;

        public Color coverColor = new Color(58, 44, 33);

    }

}
