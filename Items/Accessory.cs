using UnityEngine;

namespace AF
{

    [CreateAssetMenu(menuName = "Items / Accessory / New Accessory")]
    public class Accessory : ArmorBase
    {
        public int healthBonus = 0;
        public int staminaBonus = 0;

        public LocalizedText smallEffectDescription;

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
