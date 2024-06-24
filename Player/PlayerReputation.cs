using System;
using UnityEngine;
using UnityEngine.Localization.Settings;
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
                String.Format(
                    LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "You won reputation: +{0} points! Current reputation: {1}"),
                    value,
                    playerStatsDatabase.GetCurrentReputation()
                ),
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
                String.Format(
                    LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "You lost reputation: -{0} points! Current reputation: {1}"),
                    value,
                    playerStatsDatabase.GetCurrentReputation()
                ),
                 notificationManager.reputationDecreaseSprite);
        }
    }
}
