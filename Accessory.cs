using UnityEngine;

namespace AF
{

    [CreateAssetMenu(menuName = "Accessory / New Accessory")]
    public class Accessory : Item
    {
        public int healthBonus = 0;
        public int staminaBonus = 0;


        public void OnEquip()
        {
        }

        public void OnUnequip()
        {
        }

    }

}
