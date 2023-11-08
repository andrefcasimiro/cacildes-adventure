using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace AF
{
    [System.Serializable]
    public class LocalizedTextEntry
    {
        public GamePreferences.GameLanguage gameLanguage;
        [TextArea(5, 15)] public string text;
    }

    [System.Serializable]
    public class LocalizedText
    {
        public LocalizedTextEntry[] localizedTexts;
        Dictionary<GamePreferences.GameLanguage, string> localizedTextsDictionary = new Dictionary<GamePreferences.GameLanguage, string>();

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

            if (GamePreferences.instance == null)
            {
                return "";
            }

            if (localizedTextsDictionary.TryGetValue(GamePreferences.instance.gameLanguage, out string text))
            {
                return text;
            }
            else
            {
                Debug.Log("Key not found: for " + this);
                // Key not found, return a default value or an empty string as needed.
                return "";
            }
        }

        public string GetEnglishText()
        {
            CheckForDictionaryInitialization();

            if (GamePreferences.instance == null)
            {
                return "";
            }

            if (localizedTextsDictionary.Count <= 0)
            {
                return "";
            }

            if (localizedTextsDictionary.ContainsKey(GamePreferences.GameLanguage.ENGLISH) == false) {
                return "";
            }

            return localizedTextsDictionary[GamePreferences.GameLanguage.ENGLISH];
        }
    }
}
