using UnityEngine;

namespace AF
{

    [CreateAssetMenu(menuName = "Items / Accessory / New Accessory")]
    public class Accessory : ArmorBase
    {
        [Header("UI")]
        public LocalizedText smallEffectDescription;

        [Header("Stat Bonuses")]
        public int healthBonus = 0;
        public int staminaBonus = 0;
        public int physicalAttackBonus = 0;

        [Header("Destroy On Equip Edge Case")]
        public bool destroyOnUnequip = false;
        public AudioClip onUnequipDestroySoundclip;

        [Header("Stats")]
        public bool increaseAttackPowerTheLowerTheReputation = false;
        public bool increaseAttackPowerWithLowerHealth = false;

        [Header("Posture")]
        public int postureDamagePerParry = 0;

        [Header("Spells")]
        public bool increasesSpellDamage = false;
        public float spellDamageMultiplier = 2.15f;


        public void OnEquip()
        {
        }

        public void OnUnequip()
        {

        }

    }

}
