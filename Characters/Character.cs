using UnityEngine;
using UnityEngine.Localization.Settings;

namespace AF
{
    [CreateAssetMenu(menuName = "NPCs / Characters / New Character")]
    public class Character : ScriptableObject
    {
        public new string name;
        public string title;
        public Sprite avatar;
        public bool isPlayer = false;

        public string GetCharacterName()
        {
            if (isPlayer)
            {
                return LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "playerName");
            }

            return name;
        }

    }
}
