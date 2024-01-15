using System.Collections;
using AF.Companions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF
{
    public class EV_MarkCompanionToWait : EventBase
    {
        public CompanionID companionId;

        [Header("Components")]
        public CharacterBaseManager characterBaseManager;

        [Header("Databases")]
        public CompanionsDatabase companionsDatabase;

        public override IEnumerator Dispatch()
        {
            companionsDatabase.WaitForPlayer(companionId.companionId, new()
            {
                sceneNameWhereCompanionsIsWaitingForPlayer = SceneManager.GetActiveScene().name,
                isWaitingForPlayer = true,
                waitingPosition = characterBaseManager.transform.position,
            });

            yield return null;
        }
    }
}
