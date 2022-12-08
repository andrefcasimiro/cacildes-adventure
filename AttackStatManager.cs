using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace AF
{
    public class AttackStatManager : MonoBehaviour
    {
        // RECOMMENDED ATTACK FORMULA:
        // STR LEVEL * levelMultiplier * weaponScaling
        // A weapon that has S scaling with a levelmultiplier of 3.25 produces:
        // 1 * 3.25 * 2.4 = 8
        // 4 * 3.25 * 2.4 = 31
        // 8 * 3.25 * 2.4 = 62
        // 16 * 3.25 * 2.4 = 125
        // This gives good values, similar to Dark Souls

        [Header("Scaling Multipliers")]
        public float E = 1f;
        public float D = 1.1f;
        public float C = 1.4f;
        public float B = 1.8f;
        public float A = 2f;
        public float S = 2.4f;
        
        private Dictionary<string, float> scalingDictionary = new Dictionary<string, float>();

        [Header("Status attack bonus")]
        [Tooltip("Increased by buffs like potions, or equipment like accessories")]
        public float physicalAttackBonus = 0f;

        [Header("Physical Attack")]
        public int basePhysicalAttack = 100;
        public float levelMultiplier = 3.25f;

        private void Start()
        {
            this.scalingDictionary.Add("E", E);
            this.scalingDictionary.Add("D", D);
            this.scalingDictionary.Add("C", C);
            this.scalingDictionary.Add("B", B);
            this.scalingDictionary.Add("A", A);
            this.scalingDictionary.Add("S", S);
        }

        public int GetCurrentPhysicalAttack()
        {
            return (int)Mathf.Round(
                Mathf.Ceil(
                    basePhysicalAttack
                        + (Player.instance.strength * levelMultiplier)
                        + (Player.instance.dexterity * levelMultiplier)
                    ) + physicalAttackBonus
                );
        }


        public int GetCurrentPhysicalAttackForGivenStrengthAndDexterity(int strength, int dexterity)
        {
            return (int)Mathf.Round(
                Mathf.Ceil(
                    basePhysicalAttack
                        + (strength * levelMultiplier)
                        + (dexterity * levelMultiplier)
                    )
                );
        }


        #region Weapon Attack
        public int GetWeaponAttack(Weapon weapon)
        {
            return (int)(
                GetCurrentPhysicalAttack()
                + weapon.physicalAttack
                + GetStrengthBonusFromWeapon(weapon)
                + GetDexterityBonusFromWeapon(weapon)
            );
        }
        #endregion

        #region Scaling

        public int GetStrengthBonusFromWeapon(Weapon weapon)
        {
            return (int)(Mathf.Ceil((Player.instance.strength * this.levelMultiplier * this.scalingDictionary[weapon.strengthScaling.ToString()])));
        }

        public float GetDexterityBonusFromWeapon(Weapon weapon)
        {
            return (int)(Mathf.Ceil((Player.instance.dexterity * this.levelMultiplier * this.scalingDictionary[weapon.dexterityScaling.ToString()])));
        }

        #endregion


    }
}
