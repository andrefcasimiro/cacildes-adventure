using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace AF.Loading
{
    [CreateAssetMenu(menuName = "System / New Loading Screen")]

    public class LoadingScreen : ScriptableObject
    {
        public Sprite image;
        [TextAreaAttribute(minLines: 5, maxLines: 10)] public string text;

        [Header("Settings")]
        public string[] mapNames;


        [Header("Localization")]
        public LocalizedString text_localized;

        public string GetDisplayText()
        {
            if (text_localized != null && text_localized.IsEmpty == false)
            {
                return text_localized.GetLocalizedString();
            }

            return text;
        }


    }
}
