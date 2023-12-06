using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace AF
{
    public enum GameLanguage
    {
        PORTUGUESE,
        ENGLISH,
    }
    [System.Serializable]
    public class LocalizedTextEntry
    {
        public GameLanguage gameLanguage;
        [TextArea(5, 15)] public string text;
    }

    [System.Serializable]
    public class LocalizedText
    {
        public LocalizedTextEntry[] localizedTexts;
        Dictionary<GameLanguage, string> localizedTextsDictionary = new Dictionary<GameLanguage, string>();

        public void CheckForDictionaryInitialization()
        {
            if (localizedTextsDictionary.Count <= 0)
            {
                // Initialize table
                foreach (var localizedText in localizedTexts)
                {
                    localizedTextsDictionary.Add(localizedText.gameLanguage, localizedText.text);
                }
            }
        }

        public string GetText()
        {
            CheckForDictionaryInitialization();
            return "";
        }

        public string GetEnglishText()
        {
            CheckForDictionaryInitialization();

            if (localizedTextsDictionary.Count <= 0)
            {
                return "";
            }


            return localizedTextsDictionary[GameLanguage.ENGLISH];
        }
    }
}
