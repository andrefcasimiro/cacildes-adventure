
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Ladders
{
    public class ExitLadderTrigger : MonoBehaviour
    {

        [Header("Settings")]
        public bool isBottomExit = false;

        [Header("Events")]
        public UnityEvent onLadderExit;
        public float onLadderExit_Delay = 2f;
        PlayerManager playerManager;

        PlayerManager GetPlayerManager()
        {
            if (playerManager == null)
            {
                playerManager = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include);
            }

            return playerManager;
        }

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

            if (GetPlayerManager().climbController.climbState != ClimbState.NONE)
            {
                if (isBottomExit)
                {
                    GetPlayerManager().climbController.ExitToBottom();
                }
                else
                {
                    GetPlayerManager().climbController.ExitToTop();
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
