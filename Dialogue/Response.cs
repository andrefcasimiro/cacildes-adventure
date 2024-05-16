using AF.Flags;
using AF.Reputation;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Dialogue
{

    public class Response : MonoBehaviour
    {

        [TextArea] public string text;

        [Header("Unity Events")]
        public UnityEvent onResponseSelected;
        public UnityEvent onResponseFinished;

        [Header("Use Sub Events")]
        public GameObject subEventPage;

        [Header("Use Quick Reply")]
        public AF.Character replier;
        [TextArea] public string reply;

        [Header("Reputation")]
        public int reputationAmountToIncrease = 0;
        public int reputationAmountToDecrease = 0;
        public MonoBehaviourID reputationToAwardMonobehaviourID;

        public void AwardReputation(FlagsDatabase flagsDatabase, PlayerReputation playerReputation)
        {
            if (flagsDatabase == null)
            {
                return;
            }

            if (reputationToAwardMonobehaviourID == null)
            {
                Debug.Log("Assign a reputationToAwardMonobehaviourID to this response");
                return;
            }

            if (reputationAmountToIncrease == 0 && reputationAmountToDecrease == 0)
            {
                return;
            }

            if (reputationAmountToIncrease > 0)
            {
                playerReputation.IncreaseReputation(reputationAmountToIncrease);
            }
            else if (reputationAmountToDecrease > 0)
            {
                playerReputation.DecreaseReputation(reputationAmountToDecrease);
            }

            flagsDatabase.AddFlag(reputationToAwardMonobehaviourID);
        }

    }

}