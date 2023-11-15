using UnityEngine;
using UnityEngine.Events;

namespace AF.Triggers
{

    public class OnMonobehaviourEvents : MonoBehaviour
    {
        public UnityEvent onAwake;
        public UnityEvent onStart;

        private void Awake()
        {
            onAwake?.Invoke();
        }
        private void Start()
        {
            onStart?.Invoke();
        }

    }

}