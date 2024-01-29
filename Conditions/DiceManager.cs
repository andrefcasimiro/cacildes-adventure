using UnityEngine;
using UnityEngine.Events;

namespace AF.Conditions
{
    public class DiceManager : MonoBehaviour
    {
        public UnityEvent onSuccess;

        [Range(0, 1f)] public float chance = 0.5f;

        public void Evaluate()
        {
            if (Random.Range(0, 1f) >= chance)
            {
                onSuccess?.Invoke();
            }
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="newValue"></param>
        public void SetChance(float newValue)
        {
            chance = newValue;
        }
    }
}
