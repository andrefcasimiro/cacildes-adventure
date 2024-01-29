using UnityEngine;
namespace AF.Reputation
{

    public class PlayerReputation : MonoBehaviour
    {
        [Header("Components")]
        public PlayerStatsDatabase playerStatsDatabase;
        public Soundbank soundbank;
        public NotificationManager notificationManager;

        [Header("Achievements")]
        public Achievement negativeReputationAchievement;

        public void IncreaseReputation(int value)
        {
            playerStatsDatabase.reputation += value;

            soundbank.PlaySound(soundbank.reputationIncreased);
            notificationManager.ShowNotification(
                "You won reputation: +" + value + " points! Current reputation: " + playerStatsDatabase.GetCurrentReputation(),
                notificationManager.reputationIncreaseSprite);
        }

        public void DecreaseReputation(int value)
        {
            playerStatsDatabase.reputation -= value;

            if (playerStatsDatabase.GetCurrentReputation() <= -25)
            {
                negativeReputationAchievement.AwardAchievement();
            }

            soundbank.PlaySound(soundbank.reputationDecreased);
            notificationManager.ShowNotification(
                 "You lost reputation: -" + value + " points! Current reputation: " + playerStatsDatabase.GetCurrentReputation(),
                 notificationManager.reputationDecreaseSprite);
        }
    }
}
