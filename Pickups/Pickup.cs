using AF.Events;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace AF.Pickups
{
    public class Pickup : MonoBehaviour
    {
        [Header("Components")]
        public MonoBehaviourID monoBehaviourID;

        [Header("Systems")]
        public PickupDatabase pickupDatabase;

        [Header("Events")]
        public UnityEvent onAlreadyPickedUp;

        [Header("Replenishable Settings")]
        public int daysToReplenish = 0;
        public UnityEvent onReplenish;

        private void Awake()
        {
            EventManager.StartListening(EventMessages.ON_HOUR_CHANGED, EvaluateReplenishableState);
        }

        public void OnEnable()
        {
            if (pickupDatabase == null || monoBehaviourID == null)
            {
                return;
            }

            if (IsReplenishable() && pickupDatabase.ContainsReplenishable(monoBehaviourID.ID)
                || pickupDatabase.ContainsPickup(monoBehaviourID.ID))
            {
                onAlreadyPickedUp?.Invoke();
                return;
            }
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void UpdatePickupDatabase()
        {
            if (IsReplenishable())
            {
                pickupDatabase.AddReplenishable(monoBehaviourID.ID, daysToReplenish);
                return;
            }

            pickupDatabase.AddPickup(monoBehaviourID.ID, SceneManager.GetActiveScene().name + "-" + name);
        }

        bool IsReplenishable()
        {
            return daysToReplenish > 0;
        }

        void EvaluateReplenishableState()
        {
            if (!IsReplenishable())
            {
                return;
            }

            if (pickupDatabase.ContainsReplenishable(monoBehaviourID.ID))
            {
                onAlreadyPickedUp?.Invoke();
                return;
            }

            onReplenish?.Invoke();
        }
    }
}
