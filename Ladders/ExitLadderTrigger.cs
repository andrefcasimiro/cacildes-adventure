
using System.Collections;
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
        public float onLadderExit_Delay = 2f;

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

                StartCoroutine(OnLadderExit_Coroutine());
            }
        }

        IEnumerator OnLadderExit_Coroutine()
        {
            yield return new WaitForSeconds(onLadderExit_Delay);

            onLadderExit?.Invoke();
        }
    }
}
