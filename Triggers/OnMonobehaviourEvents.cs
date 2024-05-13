using AF.Events;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Triggers
{

    public class OnMonobehaviourEvents : MonoBehaviour
    {
        public UnityEvent onAwake;
        public UnityEvent onStart;
        public UnityEvent onEnable;
        public UnityEvent onPlayerLeaveBonfire;

        private void Awake()
        {
            onAwake?.Invoke();

            EventManager.StartListening(EventMessages.ON_LEAVING_BONFIRE, () =>
            {
                onPlayerLeaveBonfire?.Invoke();
            });
        }
        private void Start()
        {
            onStart?.Invoke();
        }
        private void OnEnable()
        {
            onEnable?.Invoke();
        }

    }
}
