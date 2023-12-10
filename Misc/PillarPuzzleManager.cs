using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AF.Music;
using UnityEngine;

namespace AF
{
    public class PillarPuzzleManager : MonoBehaviour
    {
        [Header("Components")]
        public BGMManager bgmManager;
        public int currentScore = 0;
        public int maxScore = 4;

        List<PillarPuzzleEntry> pillarPuzzleEntries = new();

        [Header("Switch to Activate On Puzzle Solve")]
        public SwitchEntry switchToActivate;

        public AudioClip successSound;
        public AudioClip failSound;

        public LocalizedText successText;

        private void Awake()
        {
            pillarPuzzleEntries = FindObjectsOfType<PillarPuzzleEntry>(true).ToList();
        }

        public void IncreaseScore()
        {
            currentScore++;

            if (currentScore >= maxScore)
            {
                var notif = FindObjectOfType<NotificationManager>(true);
                notif.ShowNotification(successText.GetEnglishText(), notif.door);

                bgmManager.PlaySound(successSound, null);

                SwitchManager.instance.UpdateSwitch(switchToActivate, true, null);
            }
        }

        public void ResetPuzzle()
        {
            currentScore = 0;

            bgmManager.PlaySound(failSound, null);

            foreach (var puzzleEntry in pillarPuzzleEntries)
            {
                puzzleEntry.ResetState();
            }
        }

    }
}
