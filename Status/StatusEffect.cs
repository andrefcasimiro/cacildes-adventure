using UnityEngine;
using UnityEngine.Localization;

namespace AF
{
    [CreateAssetMenu(menuName = "Misc / Status / New Status")]
    [System.Serializable]
    public class StatusEffect : ScriptableObject
    {
        public LocalizedString displayName;
        public LocalizedString displayNameWhenApplied;

        public string builtUpName;
        public string appliedName;
        public Sprite icon;
        public Color barColor;
        public bool isPositive = false;
        public bool isAppliedImmediately = false;
        public float decreaseRateWithDamage = 1f;
        public float decreaseRateWithoutDamage = 5f;


        public string GetName()
        {
            if (displayName == null || displayName.IsEmpty)
            {
                return builtUpName;
            }

            return displayName.GetLocalizedString();
        }

        public string GetAppliedName()
        {
            if (displayNameWhenApplied == null || displayNameWhenApplied.IsEmpty)
            {
                return appliedName;
            }

            return displayNameWhenApplied.GetLocalizedString();
        }

    }
}