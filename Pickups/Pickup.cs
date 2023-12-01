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

        public void OnEnable()
        {
            if (pickupDatabase.Contains(monoBehaviourID.ID))
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
            pickupDatabase.Add(monoBehaviourID.ID, SceneManager.GetActiveScene().name + "-" + name);
        }
    }
}
