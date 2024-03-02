using System.Collections;
using AF.Companions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF
{
    public class EV_MarkCompanionToWait : EventBase
    {
        [Header("Components")]
        public CharacterManager characterManager;

        [Header("Databases")]
        public CompanionsDatabase companionsDatabase;

        public override IEnumerator Dispatch()
        {
            companionsDatabase.WaitForPlayer(characterManager.GetCharacterID(), new()
            {
                sceneNameWhereCompanionsIsWaitingForPlayer = SceneManager.GetActiveScene().name,
                isWaitingForPlayer = true,
                waitingPosition = characterManager.transform.position,
            });

            yield return null;
        }
    }
}
