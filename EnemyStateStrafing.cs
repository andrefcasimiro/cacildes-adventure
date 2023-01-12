using AF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateStrafing : StateMachineBehaviour
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
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var lookRotation = enemyManager.player.transform.position - enemyManager.transform.position;
        var rotation = Quaternion.LookRotation(lookRotation);
        enemyManager.transform.rotation = rotation;

        enemyManager.CheckForDodgeChance();

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

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
