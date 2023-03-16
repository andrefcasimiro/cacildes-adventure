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

        [Header("Destroy On Equip Edge Case")]
        public bool destroyOnUnequip = false;
        public AudioClip onUnequipDestroySoundclip;

        public void OnEquip()
        {
        }

        public void OnUnequip()
        {
        }

    }

}
