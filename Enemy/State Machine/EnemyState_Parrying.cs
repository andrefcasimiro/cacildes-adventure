using AF;
using UnityEngine;

public class EnemyState_Parrying : StateMachineBehaviour
{
    EnemyManager enemyManager;

    

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.TryGetComponent<EnemyManager>(out enemyManager);

        if (enemyManager == null)
        {
            enemyManager = animator.GetComponentInParent<EnemyManager>();
        }
    }

}
