using UnityEngine;

namespace AF
{

    public class ShowShield : StateMachineBehaviour
    {
        Enemy enemy;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<Enemy>(out enemy);

            EnemyCombatManager enemyCombatManager = enemy.GetComponent<EnemyCombatManager>();

            if (enemyCombatManager == null)
            {
                return;
            }

            if (enemyCombatManager.shield != null)
            {
                enemyCombatManager.shield.gameObject.SetActive(true);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            EnemyCombatManager enemyCombatManager = enemy.GetComponent<EnemyCombatManager>();

            if (enemyCombatManager == null)
            {
                return;
            }

            if (enemyCombatManager.shield != null)
            {
                enemyCombatManager.shield.gameObject.SetActive(false);
            }
        }
    }

}
