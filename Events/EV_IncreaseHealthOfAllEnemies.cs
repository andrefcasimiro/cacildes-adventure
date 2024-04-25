using System.Collections;
using AF.Health;
using UnityEngine;

namespace AF
{
    public class EV_IncreaseHealthOfAllEnemies : EventBase
    {
        CharacterHealth[] characterHealths;

        public int bonusHealth = 25;

        public override IEnumerator Dispatch()
        {
            GetCharacterHealths();

            foreach (CharacterHealth characterHealth in characterHealths)
            {
                characterHealth.bonusHealth += bonusHealth;
            }

            yield return null;
        }

        void GetCharacterHealths()
        {
            if (characterHealths == null || characterHealths.Length <= 0)
            {
                characterHealths = FindObjectsByType<CharacterHealth>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            }
        }
    }

}
