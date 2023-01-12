using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Credits / New Credit")]
    public class Credits : ScriptableObject
    {
        [System.Serializable]
        public class CreditEntry
        {
            public string author;
            public string contribution;
            public string authorUrl;
            public Sprite urlSprite;
        }

        [System.Serializable]
        public class CreditsSection
        {
            public string sectionTitle;
            public CreditEntry[] creditEntry;
        }

        public CreditsSection[] creditsSections;
    }

}
