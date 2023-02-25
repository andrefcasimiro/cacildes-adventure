using UnityEngine;

namespace AF
{
    public class DoorPuzzle : SwitchListener, ISaveable
    {
        public GameObject leverInactive;
        public GameObject leverActive;
        public GameObject key;
        public GameObject door;

        NotificationManager notificationManager = FindObjectOfType<NotificationManager>(true);

        [Header("Localization")]
        public LocalizedText doorOpenedText;

        private void Start()
        {
            Refresh();
        }

        public void OnGameLoaded(GameData gameData)
        {
            Refresh();
        }

        public void Unlock()
        {
            SwitchManager.instance.UpdateSwitch(switchEntry, true);

            Soundbank.instance.PlayLeverActivated();

            notificationManager.ShowNotification(doorOpenedText.GetText(), notificationManager.door);
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

            door.SetActive(!puzzleIsSolved);
        }

    }

}
