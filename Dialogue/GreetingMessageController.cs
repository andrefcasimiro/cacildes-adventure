using UnityEngine;

namespace AF.Dialogue

{
    public class GreetingMessageController : MonoBehaviour
    {
        [TextAreaAttribute(minLines: 5, maxLines: 10)]
        public string idleGreeting = "";

        [TextAreaAttribute(minLines: 5, maxLines: 10)]
        public string receivingDamagGreeting = "";

        [Header("Components")]
        public GreetingMessage greetingMessage;

        //Flags
        bool hasDisplayedIdleMessage = false;
        bool hasDisplayedReceivingDamageMessage = false;

        public void HideGreetingMessage()
        {
            greetingMessage.Hide();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void DisplayGreetingMessage()
        {
            if (hasDisplayedIdleMessage)
            {
                HideGreetingMessage();
                return;
            }
            hasDisplayedIdleMessage = true;

            greetingMessage.greetingMessage = idleGreeting;
            greetingMessage.Display();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void DisplayReceivingDamageMessage()
        {
            if (hasDisplayedReceivingDamageMessage)
            {
                HideGreetingMessage();
                return;
            }
            hasDisplayedReceivingDamageMessage = true;

            greetingMessage.greetingMessage = receivingDamagGreeting;
        }


    }
}