using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Dialogue
{
    public class GreetingMessageController : MonoBehaviour
    {
        [Header("Greetings")]
        public CharacterGreeting[] characterGreetings;
        public float timeBeforeDisplayingAgain = 15f;
        Coroutine DisplayAgainCoroutine;

        [Header("Components")]
        public CharacterManager characterManager;
        public GreetingMessageUI greetingMessageUI;

        [Header("Events")]
        public UnityEvent onGreetingBegin;
        public UnityEvent onGreetingEnd;

        [Header("Delay Before Showing Greeting")]
        public float minimumDelayBeforeShowingGreeting = 0f;
        public float maximumDelayBeforeShowingGreeting = 0f;

        [Header("Taunt Options")]
        public bool displayMessageWhenAgressive = false;

        bool hasDisplayed = false;
        bool hasStoppedDisplaying = false;

        Coroutine ShowGreetingMessageCoroutine;
        Coroutine HideGreetingMessageCoroutine;


        private void Awake()
        {
            if (characterManager != null && characterManager.targetManager != null)
            {
                characterManager.targetManager.onAgressiveTowardsPlayer += (isAgressive) =>
                {
                    if (isAgressive)
                    {
                        if (displayMessageWhenAgressive)
                        {
                            DisplayGreetingMessage();
                        }
                        else
                        {
                            StopDisplayingGreetingMessage();
                        }
                    }
                    else
                    {
                        ResetIsDisplayed();
                    }
                };
            }
        }


        /// <summary>
        /// Unity Event
        /// </summary>
        public void StopDisplayingGreetingMessage()
        {
            if (characterGreetings == null || characterGreetings.Length <= 0 || hasStoppedDisplaying)
            {
                return;
            }

            hasStoppedDisplaying = true;
            HideGreetingMessage();
        }

        public void HideGreetingMessage()
        {
            onGreetingEnd?.Invoke();
            greetingMessageUI?.Hide();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void DisplayGreetingMessage(CharacterGreeting greetingMessage)
        {
            hasDisplayed = true;

            if (ShowGreetingMessageCoroutine != null)
            {
                StopCoroutine(ShowGreetingMessageCoroutine);
            }

            ShowGreetingMessageCoroutine = StartCoroutine(DisplayGreeting_Coroutine(greetingMessage));
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void DisplayGreetingMessage()
        {
            if (hasStoppedDisplaying || hasDisplayed || characterGreetings == null || characterGreetings.Length <= 0)
            {
                return;
            }

            var greetingMessage =
                characterGreetings.FirstOrDefault(messageGameObject => messageGameObject != null && messageGameObject.isActiveAndEnabled);

            if (greetingMessage == null)
            {
                return;
            }

            DisplayGreetingMessage(greetingMessage);
        }

        IEnumerator DisplayGreeting_Coroutine(CharacterGreeting characterGreeting)
        {
            yield return new WaitForSeconds(Random.Range(minimumDelayBeforeShowingGreeting, maximumDelayBeforeShowingGreeting));

            onGreetingBegin?.Invoke();

            greetingMessageUI.Display(characterGreeting.greeting);

            if (HideGreetingMessageCoroutine != null)
            {
                StopCoroutine(HideGreetingMessageCoroutine);
            }

            HideGreetingMessageCoroutine = StartCoroutine(HideGreetingMessage_Coroutine(characterGreeting.duration));
        }

        IEnumerator HideGreetingMessage_Coroutine(float duration)
        {
            yield return new WaitForSeconds(duration);

            HideGreetingMessage();

            if (DisplayAgainCoroutine != null)
            {
                StopCoroutine(DisplayAgainCoroutine);
            }

            DisplayAgainCoroutine = StartCoroutine(ResetIsDiplayed_Coroutine());
        }

        IEnumerator ResetIsDiplayed_Coroutine()
        {
            yield return new WaitForSeconds(timeBeforeDisplayingAgain);
            hasDisplayed = false;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ResetIsDisplayed()
        {
            if (DisplayAgainCoroutine != null)
            {
                StopCoroutine(DisplayAgainCoroutine);
            }

            hasDisplayed = false;
        }

    }
}
