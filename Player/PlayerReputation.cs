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
            playerStatsDatabase.IncreaseReputation(value);

            soundbank.PlaySound(soundbank.reputationIncreased);
            notificationManager.ShowNotification(
                "You won reputation: +" + value + " points! Current reputation: " + playerStatsDatabase.GetCurrentReputation(),
                notificationManager.reputationIncreaseSprite);
        }

        public void DecreaseReputation(int value)
        {
            playerStatsDatabase.DecreaseReputation(value);

            if (playerStatsDatabase.GetCurrentReputation() <= -15)
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
