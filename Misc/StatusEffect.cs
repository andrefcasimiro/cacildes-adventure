using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Misc / Status / New Status")]
    [System.Serializable]
    public class StatusEffect : ScriptableObject
    {
        public string builtUpName;
        public string appliedName;
        public Sprite icon;
        public Color barColor;
        public bool isPositive = false;
        public bool isAppliedImmediately = false;
        public float decreaseRateWithDamage = 1f;
        public float decreaseRateWithoutDamage = 5f;
    }
}