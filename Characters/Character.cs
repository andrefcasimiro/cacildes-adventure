using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace AF
{
    [CreateAssetMenu(menuName = "NPCs / Characters / New Character")]
    public class Character : ScriptableObject
    {
        [Header("Name")]
        public new string name;

        public LocalizedString name_Localized;

        [Header("Title")]
        public string title;
        public LocalizedString title_Localized;

        [Header("Graphics")]
        public Sprite avatar;
        public bool isPlayer = false;

        public string GetCharacterName()
        {
            if (isPlayer)
            {
                return LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "playerName");
            }

            if (name_Localized != null && name_Localized.IsEmpty == false)
            {
                return name_Localized.GetLocalizedString();
            }

            return name;
        }

        public string GetCharacterTitle()
        {
            if (title_Localized != null && title_Localized.IsEmpty == false)
            {
                return title_Localized.GetLocalizedString();
            }

            return title;
        }

    }
}
