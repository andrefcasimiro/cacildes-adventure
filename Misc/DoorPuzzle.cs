using UnityEngine;

namespace AF
{
    public class DoorPuzzle : SwitchListener
    {
        public GameObject leverInactive;
        public GameObject leverActive;
        public GameObject key;
        public GameObject door;

        [Header("Components")]
        public NotificationManager notificationManager;
        public Soundbank soundbank;

        [Header("Localization")]
        public LocalizedText doorOpenedText;

        private void Start()
        {
        }

        public void Unlock()
        {
            SwitchManager.instance.UpdateSwitch(switchEntry, true, null);

            soundbank.PlaySound(soundbank.activateLever);

            if (doorOpenedText != null && doorOpenedText.localizedTexts.Length > 0)
            {
                notificationManager.ShowNotification(doorOpenedText.GetEnglishText(), notificationManager.door);
            }
        }


        public override void Refresh()
        {
            bool puzzleIsSolved = SwitchManager.instance.GetSwitchCurrentValue(switchEntry);

            if (key != null)
            {
                key.SetActive(false);
            }
            else if (leverActive != null && leverInactive != null)
            {
                leverActive.SetActive(puzzleIsSolved);
                leverInactive.SetActive(!puzzleIsSolved);
            }

            if (door != null)
            {
                door.SetActive(!puzzleIsSolved);
            }
        }

    }

}
