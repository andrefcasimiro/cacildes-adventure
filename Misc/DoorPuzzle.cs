using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class DoorPuzzle : SwitchListener, ISaveable
    {
        public GameObject leverInactive;
        public GameObject leverActive;

        public GameObject key;

        public GameObject door;

        NotificationManager notificationManager;

        private void Awake()
        {
            this._switch = SwitchManager.instance.GetSwitchInstance(this.switchUuid);

        }

        private void Start()
        {

            notificationManager = FindObjectOfType<NotificationManager>(true);

            EvaluateSwitch();
        }

        public void OnGameLoaded(GameData gameData)
        {
            EvaluateSwitch();
        }

        public void Unlock()
        {
            SwitchManager.instance.UpdateSwitch(this.switchUuid, true);

            Soundbank.instance.PlayLeverActivated();

            notificationManager.ShowNotification("A door was opened somewhere", notificationManager.door);
        }


        public override void EvaluateSwitch()
        {
            bool puzzleIsSolved = SwitchManager.instance.GetSwitchValue(this.switchUuid);

            if (key != null)
            {
                key.gameObject.SetActive(false);
            }
            else if (leverActive != null && leverInactive != null)
            {
                leverActive.gameObject.SetActive(puzzleIsSolved);
                leverInactive.gameObject.SetActive(!puzzleIsSolved);
            }

            door.gameObject.SetActive(!puzzleIsSolved);
        }

    }

}
