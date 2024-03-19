using AF.Events;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class CharacterSleepController : MonoBehaviour
    {
        [Header("Settings")]
        public bool canSleep = false;
        public float sleepFrom = 22f;
        public float sleepUntil = 05f;

        [Header("Flags")]
        public bool isSleeping = false;

        [Header("Events")]
        public UnityEvent onBeginSleep;
        public UnityEvent onWakeUp;

        [Header("Components")]
        public CharacterManager characterManager;

        [Header("Systems")]
        public GameSession gameSession;

        private void Awake()
        {
            gameObject.SetActive(canSleep);
        }

        void Start()
        {
            OnHourChanged();

            EventManager.StartListening(EventMessages.ON_HOUR_CHANGED, OnHourChanged);
        }

        public void OnHourChanged()
        {
            if (canSleep == false)
            {
                return;
            }

            bool shouldSleep;

            // If appear until is after midnight, it may become smaller than appearFrom (i. e. appear from 17 until 4)
            if (sleepFrom > sleepUntil)
            {
                shouldSleep = gameSession.timeOfDay >= sleepFrom && gameSession.timeOfDay <= 24 || (gameSession.timeOfDay >= 0 && gameSession.timeOfDay <= sleepUntil);
            }
            else
            {
                shouldSleep = gameSession.timeOfDay >= sleepFrom && gameSession.timeOfDay <= sleepUntil;
            }

            if (isSleeping && !shouldSleep)
            {
                WakeUp();
            }
            else if (shouldSleep && CanSleep())
            {
                Sleep();
            }
        }

        bool CanSleep()
        {
            if (characterManager.isBusy)
            {
                return false;
            }

            if (characterManager.targetManager.currentTarget != null)
            {
                return false;
            }

            return true;
        }

        public void Sleep()
        {
            isSleeping = true;

            onBeginSleep?.Invoke();
        }

        public void WakeUp()
        {
            if (!isSleeping)
            {
                return;
            }

            isSleeping = false;

            onWakeUp?.Invoke();
        }

    }
}
