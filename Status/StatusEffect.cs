using UnityEngine;
using System.Linq;

namespace AF
{
    [CreateAssetMenu(menuName = "Status / New Status")]
    public class StatusEffect : ScriptableObject
    {
        public string name;

        public Sprite spriteIndicator;

        public Color barColor;

        public Stat statAffected = Stat.None;

        // Will be influenced by the resistance of the player
        public float maxAmountBeforeDamage = 100;

        public float damagePerSecond = 1;

        public float decreaseRateWithoutDamage = 5;
        public float decreaseRateWithDamage = 1;

        [Header("FX")]
        public GameObject particleOnDamage;

    }

}
