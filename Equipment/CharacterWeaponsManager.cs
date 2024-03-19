using System.Linq;
using UnityEngine;

namespace AF.Equipment
{
    public class CharacterWeaponsManager : MonoBehaviour
    {

        public CharacterWeaponHitbox[] weapons;
        public GameObject bow;
        public GameObject shield;
        public bool shouldHideShield = true;

        public void ResetStates()
        {
            CloseAllWeaponHitboxes();
        }

        public void ShowEquipment()
        {
            ShowWeapon();
            ShowBow();
            ShowShield();
        }

        public void HideEquipment()
        {
            HideWeapon();
            HideBow();
            HideShield();
        }

        public void ShowWeapon()
        {
            if (weapons.Length > 0)
            {
                foreach (var weapon in weapons)
                {
                    weapon.gameObject.SetActive(true);
                }
            }
        }
        public void HideWeapon()
        {
            if (weapons.Length > 0)
            {
                foreach (var weapon in weapons)
                {
                    weapon.gameObject.SetActive(false);
                }
            }
        }

        public void ShowShield()
        {
            if (shield != null)
            {
                shield.SetActive(true);
            }
        }
        public void HideShield()
        {
            if (shield != null && shouldHideShield)
            {
                shield.SetActive(false);
            }
        }

        public void ShowBow()
        {
            if (bow != null)
            {
                bow.SetActive(true);
            }
        }

        public void HideBow()
        {
            if (bow != null)
            {
                bow.SetActive(false);
            }
        }

        public void OpenCharacterWeaponHitbox()
        {
            if (weapons.Length > 0)
            {
                OpenCharacterWeaponHitbox(weapons[0]);
            }
        }

        public void CloseCharacterWeaponHitbox()
        {
            if (weapons.Length > 0)
            {
                CloseCharacterWeaponHitbox(weapons[0]);
            }
        }

        public void OpenCharacterWeaponHitbox(CharacterWeaponHitbox characterWeaponHitbox)
        {
            characterWeaponHitbox?.EnableHitbox();
        }

        public void CloseCharacterWeaponHitbox(CharacterWeaponHitbox characterWeaponHitbox)
        {
            characterWeaponHitbox?.DisableHitbox();
        }

        public void CloseAllWeaponHitboxes()
        {
            foreach (CharacterWeaponHitbox characterWeaponHitbox in weapons)
            {
                characterWeaponHitbox?.DisableHitbox();
            }
        }
    }
}
