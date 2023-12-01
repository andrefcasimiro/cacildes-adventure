
using UnityEngine;
using UnityEngine.Events;

namespace AF.Ladders
{
    public class ExitLadderTrigger : MonoBehaviour
    {
        [Header("Components")]
        public PlayerManager playerManager;

        [Header("Settings")]
        public bool isBottomExit = false;

        [Header("Events")]
        public UnityEvent onLadderExit;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            if (playerManager.climbController.climbState != ClimbState.NONE)
            {
                if (isBottomExit)
                {
                    playerManager.climbController.ExitToBottom();
                }
                else
                {
                    playerManager.climbController.ExitToTop();
                }

                onLadderExit?.Invoke();
            }
        }
    }
}
