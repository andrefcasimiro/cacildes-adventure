using System.Collections;
using AF.Companions;
using UnityEngine;

namespace AF
{
    public class EV_MarkCompanionToFollow : EventBase
    {
        public CharacterManager characterManager;

        [Header("Databases")]
        public CompanionsDatabase companionsDatabase;

        public override IEnumerator Dispatch()
        {
            companionsDatabase.FollowPlayer(characterManager.GetCharacterID());

            yield return null;
        }
    }
}
