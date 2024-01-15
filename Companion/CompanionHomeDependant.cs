using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF.Companions
{
    public class CompanionHomeDependant : MonoBehaviour
    {
        public CompanionID companionId;

        public bool sceneIsHome = false;

        [Header("Databases")]
        public CompanionsDatabase companionsDatabase;

        private void Start()
        {
            Evaluate();
        }

        public void Evaluate()
        {
            bool isActive = false;

            // If in party and actively following, always appear on every map
            if (companionsDatabase.IsCompanionAndIsActivelyInParty(companionId.companionId))
            {
                isActive = true;
            }
            // If companion is waiting in the active scene, show him
            else if (companionsDatabase.IsCompanionWaiting(companionId.companionId))
            {
                CompanionState companionState = companionsDatabase.GetWaitState(companionId.companionId);

                if (companionState.sceneNameWhereCompanionsIsWaitingForPlayer == SceneManager.GetActiveScene().name)
                {
                    companionId.SpawnCompanion(companionState.waitingPosition);
                    isActive = true;
                }
            }
            // Companion not in party and scene is home
            else if (sceneIsHome && companionsDatabase.IsInParty(companionId.companionId) == false)
            {
                isActive = true;
            }

            Utils.UpdateTransformChildren(
                transform,
                isActive
            );
        }
    }
}
