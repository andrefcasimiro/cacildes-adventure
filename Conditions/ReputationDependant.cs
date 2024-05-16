using AF.Companions;
using AF.Events;
using TigerForge;
using UnityEngine;

namespace AF.Conditions
{
    public class ReputationDependant : MonoBehaviour
    {
        public PlayerStatsDatabase playerStatsDatabase;
        public bool shouldCountUpwards = true;
        public bool shouldCountDownwards = false;
        public int minimumReputationRequired = 0;

        private void Start()
        {
            Evaluate();

            EventManager.StartListening(EventMessages.ON_REPUTATION_CHANGED, Evaluate);
        }

        public void Evaluate()
        {
            bool isActive = false;

            if (shouldCountUpwards)
            {
                isActive = playerStatsDatabase.GetCurrentReputation() >= minimumReputationRequired;
            }
            else if (shouldCountDownwards)
            {
                isActive = playerStatsDatabase.GetCurrentReputation() <= minimumReputationRequired;
            }

            Utils.UpdateTransformChildren(
                transform,
                isActive
            );
        }
    }
}
