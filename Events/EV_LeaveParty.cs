using System.Collections;
using AF.Companions;
using UnityEngine;

namespace AF
{

    public class EV_LeaveParty : EventBase
    {
        public CharacterManager characterManager;

        [Header("Databases")]
        public CompanionsDatabase companionsDatabase;

        public override IEnumerator Dispatch()
        {
            companionsDatabase.RemoveFromParty(characterManager.GetCharacterID());
            yield return null;
        }
    }
}
