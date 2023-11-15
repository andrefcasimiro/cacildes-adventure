using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AF
{
    public class CavernLightTrigger : MonoBehaviour
    {
        public Enemy[] enemiesToBuff;

        public int weaponDamageBonus = 50;

        private void OnTriggerEnter(Collider other)
        {
            if (IsEnemy(other))
            {
                var enemyCombatController = other.GetComponent<EnemyCombatController>();

                if (enemyCombatController != null)
                {
                    enemyCombatController.weaponDamageBonus = weaponDamageBonus;
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (IsEnemy(other))
            {
                var enemyCombatController = other.GetComponent<EnemyCombatController>();

                if (enemyCombatController != null)
                {
                    enemyCombatController.weaponDamageBonus = 0;
                }
            }
        }

        bool IsEnemy(Collider other)
        {

            if (other.CompareTag("Enemy"))
            {
                CharacterManager characterManager = other.GetComponent<CharacterManager>();

                if (characterManager != null)
                {
                    return false; //enemiesToBuff.Contains(characterManager.enemy);

                }
            }

            return false;
        }
    }

}
