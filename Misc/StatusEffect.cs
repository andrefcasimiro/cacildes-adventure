using UnityEngine;

namespace AF
{
    public enum Stat
    {
        None,
        Health,
        Magic,
        Stamina,
        Reputation
    }

    [CreateAssetMenu(menuName = "Misc / Status / New Status")]
    public class StatusEffect : ScriptableObject
    {
        public new LocalizedText name;

        public LocalizedText appliedStatusDisplayName; 
 
        public Sprite spriteIndicator;

        public Color barColor;

        public Stat statAffected = Stat.None;

        [Header("Damage Stat Options")]
        public bool usePercentuagelDamage = false;
        public int damagePercentualValue = 30;

        [Header("Immediate Effect Options")]
        [Tooltip("If true, the effect will be applied once the bar reaches its full, and then the effect will be removed")]
        public bool effectIsImmediate = false;

        [Header("Effect over time Options")]
        public float damagePerSecond = 1;

        [Header("FX")]
        public GameObject particleOnDamage;
    }
}
