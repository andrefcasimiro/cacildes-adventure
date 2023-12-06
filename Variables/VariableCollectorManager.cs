using AF.Music;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class VariableCollectorManager : VariableListener
    {
        [Header("Switch Conditions")]
        public SwitchEntry switchEntry;
        public bool switchValue;

        [Header("Options")]

        public int maxValue = 10;

        public UnityEvent onCollectAllEvent;

        [Header("Notifications")]
        public bool showNotification = true;
        public LocalizedText notificationText;
        public Sprite notificationSprite;
        NotificationManager notificationManager;
        public AudioClip onCollectSfx;

        [Header("Components")]
        public BGMManager bgmManager;

        private void Awake()
        {
            notificationManager = FindObjectOfType<NotificationManager>(true);
        }

        private void Start()
        {
            CheckIfMinigameWasComplete();
        }

        public void CollectOne()
        {
            Collect(1);
        }

        public void Collect(int amount)
        {
            var currentAmount = VariableManager.instance.GetVariableValue(variableEntry);
            var newAmount = currentAmount + amount;
            VariableManager.instance.UpdateVariable(variableEntry, newAmount);

            if (onCollectSfx != null)
            {
                bgmManager.PlaySound(onCollectSfx, null);
            }

            if (showNotification)
            {
                notificationManager.ShowNotification(newAmount + "/" + maxValue + " " + notificationText.GetText(), notificationSprite);
            }
        }

        public override void Refresh()
        {
            var variableValue = VariableManager.instance.GetVariableValue(variableEntry);

            if (variableValue >= maxValue)
            {
                if (!ShouldRun())
                {
                    return;
                }

                onCollectAllEvent.Invoke();
            }
        }

        public void OnGameLoaded(object gameData)
        {
            CheckIfMinigameWasComplete();
        }


        void CheckIfMinigameWasComplete()
        {
            if (!ShouldRun())
            {
                return;
            }

            if (VariableManager.instance.GetVariableValue(variableEntry) != maxValue)
            {
                VariableManager.instance.UpdateVariable(variableEntry, 0);

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
