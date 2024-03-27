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

        bool hasDisplayed = false;
        bool hasStoppedDisplaying = false;

        Coroutine HideGreetingMessageCoroutine;


        private void Awake()
        {
            if (characterManager != null && characterManager.targetManager != null)
            {
                characterManager.targetManager.onAgressiveTowardsPlayer += (isAgressive) =>
                {
                    if (isAgressive)
                    {
                        StopDisplayingGreetingMessage();
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
            HideGreetingMessage();
            hasStoppedDisplaying = true;
        }

        public void HideGreetingMessage()
        {
            onGreetingEnd?.Invoke();
            greetingMessageUI?.Hide();
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

            hasDisplayed = true;
            onGreetingBegin?.Invoke();

            greetingMessageUI.Display(greetingMessage.greeting);

            if (HideGreetingMessageCoroutine != null)
            {
                StopCoroutine(HideGreetingMessageCoroutine);
            }

            HideGreetingMessageCoroutine = StartCoroutine(HideGreetingMessage_Coroutine(greetingMessage.duration));
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
