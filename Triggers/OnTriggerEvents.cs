using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Triggers
{
    public class OnTriggerEvents : MonoBehaviour
    {
        [Header("Object detection")]
        public GameObject target;

        [Header("Tags")]
        public string[] tagsToDetect;

        [Header("Events")]
        public UnityAction<GameObject> onCharacterEnter;

        [Header("Settings")]
        public float intervalBetweenInvokations = 1f;

        [Header("Player Options")]
        public bool requirePlayerDodgeToTrigger = false;

        bool m_canTriggerOnStay = true;
        public bool CanTriggerOnStay
        {
            get
            {
                return m_canTriggerOnStay;
            }

            set
            {
                if (value == false)
                {
                    Invoke(nameof(ResetCanTrigger), intervalBetweenInvokations);
                }

                m_canTriggerOnStay = value;
            }
        }

        [Header("Events")]
        public UnityEvent onTriggerEnterEvent;
        public UnityEvent onTriggerStayEvent;
        public UnityEvent onTriggerExitEvent;

        bool isActive = true;

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="collider"></param>
        /// <returns></returns>
        public void SetIsActive(bool value)
        {
            isActive = value;
        }

        bool ShouldTrigger(Collider collider)
        {
            if (!isActive)
            {
                return false;
            }

            if (target != null && collider.gameObject.Equals(target))
            {
                return true;
            }
            else if (tagsToDetect.Contains(collider.tag))
            {
                return true;
            }

            return false;
        }

        public void OnTriggerEnter(Collider collider)
        {
            if (ShouldTrigger(collider))
            {
                if (!requirePlayerDodgeToTrigger || ShouldTriggerOnPlayerDodge(collider))
                {
                    onTriggerEnterEvent?.Invoke();
                }
            }
        }

        bool ShouldTriggerOnPlayerDodge(Collider collider)
        {
            if (collider.gameObject.TryGetComponent<PlayerManager>(out PlayerManager playerManager))
            {
                if (playerManager != null)
                {
                    return playerManager.dodgeController.isDodging;

                }
            }

            return false;
        }

        public void OnTriggerStay(Collider collider)
        {
            if (!CanTriggerOnStay)
            {
                return;
            }

            if (ShouldTrigger(collider))
            {
                if (!requirePlayerDodgeToTrigger || ShouldTriggerOnPlayerDodge(collider))
                {
                    onTriggerStayEvent?.Invoke();

                    CanTriggerOnStay = false;
                }
            }
        }

        public void OnTriggerExit(Collider collider)
        {
            if (ShouldTrigger(collider))
            {
                onTriggerExitEvent?.Invoke();
            }
        }


        void ResetCanTrigger()
        {
            CanTriggerOnStay = true;
        }

    }

}