﻿using UnityEngine;

namespace AF
{

    public class DisableOnDeath : StateMachineBehaviour
    {
        Enemy enemy;

        public bool turnKinematic = true;
        public bool disableCapsuleCollider = true;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<Enemy>(out enemy);
            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<Enemy>(true);
            }

            enemy.GetComponent<Rigidbody>().isKinematic = turnKinematic;
            enemy.GetComponent<CapsuleCollider>().enabled = !disableCapsuleCollider;


            enemy.GetComponent<EnemyHealthbox>().healthBarSlider.gameObject.SetActive(false);
            enemy.GetComponent<Healthbox>().enabled = false;
            enemy.agent.enabled = false;
            enemy.enabled = false;
        }

    }
}