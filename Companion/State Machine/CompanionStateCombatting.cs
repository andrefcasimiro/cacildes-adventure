﻿using UnityEngine;

namespace AF
{

    public class CompanionStateCombatting : StateMachineBehaviour
    {
        CompanionManager companionManager;

        public string[] attackClipNames;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (companionManager == null)
            {
                animator.gameObject.TryGetComponent<CompanionManager>(out companionManager);

                companionManager = animator.GetComponentInParent<CompanionManager>(true);
            }

            companionManager.FaceEnemy();

            companionManager.HideGenericTrigger();

            companionManager.agent.isStopped = true;

            if (companionManager.currentEnemy == null || companionManager.currentEnemy.currentHealth <= 0)
            {
                companionManager.StopCombat();
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (companionManager.ShouldChaseEnemy())
            {
                animator.Play(companionManager.hashChasing);
                return;
            }

            // Roll attack dice
            if (companionManager.CanAttack())
            {
                companionManager.ResetAttackCooldown();

                var animationDice = Random.Range(0, attackClipNames.Length);

                animator.Play(attackClipNames[animationDice]);
            }

        }

    }

}
