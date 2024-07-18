using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

namespace AF.Arena
{
    public class ArenaManagerUI : MonoBehaviour
    {
        public UIDocument uIDocument;
        VisualElement root;

        Label round;
        Label counter;
        Label bestRound;

        [Header("Unity Events")]
        public UnityAction onMinuteChanged;

        int previousMinute = -1;

        public int maxRounds = -1;

        private void OnEnable()
        {
            root = uIDocument.rootVisualElement;
            root.style.display = DisplayStyle.None;
            round = root.Q<Label>("Round");
            counter = root.Q<Label>("Counter");
            bestRound = root.Q<Label>("BestRound");
            bestRound.style.display = DisplayStyle.None;
        }

        public void UpdateCurrentRound(int currentRound)
        {
            round.text = $"{LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Round")} {currentRound} / {maxRounds}";
            root.style.display = DisplayStyle.Flex;
        }

        public void UpdateWaitingForNextRound(int timeBeforeNextRound)
        {
            round.text = $"{LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Next round will begin in")} {timeBeforeNextRound}";
            root.style.display = DisplayStyle.Flex;
        }

        public void UpdateTimerUI(float elapsedTime)
        {
            counter.text = GetFormmatedTimed(elapsedTime);
        }

        public string GetFormmatedTimed(float elapsedTime)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedTime);

            int hours = timeSpan.Hours;
            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;

            if (minutes != previousMinute && minutes >= 1)
            {
                previousMinute = minutes;

                onMinuteChanged?.Invoke();
            }

            return $"{hours}h : {minutes}m : {seconds}s";
        }
    }
}
