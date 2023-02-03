using System.Collections.Generic;

namespace AF
{
    [System.Serializable]
    public class LocalizedTextEntry
    {
        public GamePreferences.GameLanguage gameLanguage;
        public string text;
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

            return localizedTextsDictionary[GamePreferences.instance.gameLanguage];
        }
    }
}
