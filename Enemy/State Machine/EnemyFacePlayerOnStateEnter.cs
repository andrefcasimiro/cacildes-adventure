using UnityEngine;

namespace AF
{
    public class EnemyFacePlayerOnStateEnter : StateMachineBehaviour
    {
        EnemyManager enemy;

        public bool facePlayerOnStateUpdate = false;

        [Tooltip("If set, will only face player from minimum distance set")]
        public float minimumDistanceToPlayer = 0f;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent(out enemy);
            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<EnemyManager>(true);
            }

            HandleFacePlayer();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (facePlayerOnStateUpdate)
            {
                HandleFacePlayer();
            }
        }

        void HandleFacePlayer()
        {
            if (Vector3.Distance(enemy.transform.position, enemy.player.transform.position) < minimumDistanceToPlayer)
            {
                return;
            }

            enemy.facePlayer = true;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemy.facePlayer = false;
        }
    }
}
