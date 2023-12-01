using AF.Events;
using TigerForge;
using UnityEngine;

namespace AF.Conditions
{
    public abstract class ConditionBase : MonoBehaviour
    {
        public abstract bool IsConditionMet();

        private void Awake()
        {
            EventManager.StartListening(EventMessages.ON_MOMENT_START, Evaluate);
        }

        private void Start()
        {
            Evaluate();
        }

        public void Evaluate()
        {
            bool conditionResult = IsConditionMet();
            ToggleChildObjects(conditionResult);
        }

        private void ToggleChildObjects(bool conditionResult)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(conditionResult);
            }
        }
    }
}
