using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_AutoEquip : EventBase
    {
        public Weapon weapon;
        public Shield shield;
        public Armor armor;

        public override IEnumerator Dispatch()
        {
            yield return null;

            EquipmentGraphicsHandler equipmentGraphicsHandler = FindObjectOfType<EquipmentGraphicsHandler>(true);

            if (equipmentGraphicsHandler != null)
            {
                if (weapon != null)
                {
                    equipmentGraphicsHandler.Equip(weapon);
                }
                if (shield != null)
                {
                    equipmentGraphicsHandler.Equip(shield);
                }
                if (armor != null)
                {
                    equipmentGraphicsHandler.Equip(armor);
                }
            }
        }
    }

}
