using UnityEngine;

namespace AF
{
    public class EnemyDeactivateWeaponHitboxesOnExit : StateMachineBehaviour
    {
        EnemyManager enemy;

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent(out enemy);

            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<EnemyManager>(true);
            }

            enemy.enemyWeaponController.DisableAllWeaponHitboxes();
        }
    }
}
