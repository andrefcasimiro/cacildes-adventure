using UnityEngine;

namespace AF.Journals
{
    public class JournalPage : MonoBehaviour
    {
        public string pageTitle;

        [TextAreaAttribute(minLines: 20, maxLines: 999, order = 0)] public string pageText;
    }
}
