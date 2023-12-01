using System.Collections;
using AF.Shooting;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Ladders
{
    public class ClimbLadderTrigger : MonoBehaviour
    {
        [Header("Components")]
        public PlayerManager playerManager;

        [Header("Settings")]
        public bool isFromBottom = false;
        public Transform targetTransform;

        [Header("Unity Events")]
        public UnityEvent onLadderEnter;
        public UnityEvent onLadderEnter_AfterDelay;
        public float onLadderEnter_Delay = 2f;

        /// <summary>
        /// Unity Event
        /// </summary>
        public void BeginClimbing()
        {
            playerManager.climbController.StartClimbing(targetTransform, isFromBottom);
            StartCoroutine(DelayOnLadderEnter_Coroutine());
        }

        IEnumerator DelayOnLadderEnter_Coroutine()
        {
            onLadderEnter?.Invoke();
            yield return new WaitForSeconds(onLadderEnter_Delay);
            onLadderEnter_AfterDelay?.Invoke();
        }
    }
}
