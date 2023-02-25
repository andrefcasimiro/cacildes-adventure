using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class VariableCollectorManager : VariableListener, ISaveable
    {
        [Header("Switch Conditions")]
        public SwitchEntry switchEntry;
        public bool switchValue;

        [Header("Options")]

        public int maxValue = 10;

        public UnityEvent onCollectAllEvent;

        [Header("Notifications")]
        public bool showNotification = true;
        public string notificationText = "collected!";
        public Sprite notificationSprite;
        NotificationManager notificationManager;
        public AudioClip onCollectSfx;

        private void Awake()
        {
            notificationManager = FindObjectOfType<NotificationManager>(true);
        }

        private void Start() {        
            CheckIfMinigameWasComplete();
        }

        public void CollectOne()
        {
            Collect(1);

        }

        public void Collect(int amount)
        {
            var currentAmount = VariableManager.instance.GetVariableValue(variableUuid);
            var newAmount = currentAmount + amount;
            VariableManager.instance.UpdateVariable(variableUuid, newAmount);

            if (onCollectSfx != null)
            {
                BGMManager.instance.PlaySound(onCollectSfx, null);
            }
            if (showNotification)
            {
                notificationManager.ShowNotification(newAmount + "/" + maxValue + " " + notificationText, notificationSprite);
            }

        }

        public override void EvaluateVariable()
        {
            var variableValue = VariableManager.instance.GetVariableValue(variableUuid);

            if (variableValue >= maxValue)
            {
                if (!ShouldRun())
                {
                    return;
                }

                onCollectAllEvent.Invoke();
            }
        }

        public void OnGameLoaded(GameData gameData)
        {
            CheckIfMinigameWasComplete();
        }


        void CheckIfMinigameWasComplete()
        {
            if (!ShouldRun())
            {
                return;
            }

            if (VariableManager.instance.GetVariableValue(variableUuid) != maxValue)
            {
                VariableManager.instance.UpdateVariable(variableUuid, 0);

                // Activate all children

                foreach (Transform ch in this.gameObject.transform)
                {
                    ch.gameObject.SetActive(true);

                }
            }
            else
            {
                foreach (Transform ch in this.gameObject.transform)
                {
                    ch.gameObject.SetActive(false);
                }

                onCollectAllEvent.Invoke();
            }
        }

        public bool ShouldRun()
        {
            return SwitchManager.instance.GetSwitchCurrentValue(switchEntry) == switchValue;
        }
    }

}
