using AF;
using UnityEngine;

public class EnemyState_Strafing : StateMachineBehaviour
{
    public float maxTimeStrafing = 5f;
    public bool isRight = false;
    float strafeTime = 0f;

    EnemyManager enemyManager;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        strafeTime = 0f;

        animator.TryGetComponent<EnemyManager>(out enemyManager);

        if (enemyManager == null)
        {
            enemyManager = animator.GetComponentInParent<EnemyManager>();
        }

        if (enemyManager.enemyBlockController != null)
        {
            animator.SetBool(enemyManager.hashIsBlocking, enemyManager.enemyBlockController.blockWhileStrafing);
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemyManager.enemyDodgeController != null)
        {
            enemyManager.enemyDodgeController.CheckForDodgeChance();
        }

        strafeTime += Time.deltaTime;

        if (strafeTime >= maxTimeStrafing)
        {
            animator.SetBool(enemyManager.hashIsStrafing, false);
        }

        if (strafeTime > maxTimeStrafing / 2) {
            // Decide if we try attacking
            float randomDice = Random.Range(0, 1f);
            if (randomDice > 0.5f)
            {
                animator.Play(enemyManager.hashCombatting);
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset block bool on state exit
        if (enemyManager.enemyBlockController != null && enemyManager.enemyBlockController.blockWhileStrafing)
        {
            animator.SetBool(enemyManager.hashIsBlocking, false);
        }
    }
}
