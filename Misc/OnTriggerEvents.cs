using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Misc
{
    public class OnTriggerEvents : MonoBehaviour
    {
        public new string tag = "Player";
        public UnityEvent onTriggerEnterEvent;
        public UnityEvent onTriggerStayEvent;
        public UnityEvent onTriggerExitEvent;

        public bool disableOnTriggerEnter = false;
        public bool disableOnTriggerExit = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(tag))
            {
                onTriggerEnterEvent.Invoke();

                if (disableOnTriggerEnter)
                {
                    this.gameObject.SetActive(false);
                }
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag(tag))
            {
                onTriggerStayEvent.Invoke();
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(tag))
            {
                onTriggerExitEvent.Invoke();

                if (disableOnTriggerExit)
                {
                    this.gameObject.SetActive(false);
                }
            }
        }
    }
}
