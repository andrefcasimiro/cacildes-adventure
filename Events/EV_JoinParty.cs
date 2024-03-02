using System.Collections;
using AF.Companions;
using UnityEngine;

namespace AF
{

    public class EV_JoinParty : EventBase
    {
        public CharacterManager characterManager;

        [Header("Databases")]
        public CompanionsDatabase companionsDatabase;

        public override IEnumerator Dispatch()
        {
            companionsDatabase.AddToParty(characterManager.GetCharacterID());
            yield return null;
        }
    }
}
