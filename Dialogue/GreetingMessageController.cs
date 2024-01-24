using System.Collections;
using System.Linq;
using UnityEngine;

namespace AF.Dialogue
{
    public class GreetingMessageController : MonoBehaviour
    {
        [Header("Greetings")]
        public CharacterGreeting[] characterGreetings;

        [Header("Components")]
        public GreetingMessageUI greetingMessageUI;

        bool hasDisplayed = false;

        Coroutine HideGreetingMessageCoroutine;

        public void HideGreetingMessage()
        {
            greetingMessageUI.Hide();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void DisplayGreetingMessage()
        {
            if (hasDisplayed || characterGreetings == null || characterGreetings.Length <= 0)
            {
                return;
            }

            hasDisplayed = true;

            var greetingMessage =
                characterGreetings.FirstOrDefault(messageGameObject => messageGameObject.isActiveAndEnabled);

            if (greetingMessage == null)
            {
                return;
            }
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
        }

    }
}
