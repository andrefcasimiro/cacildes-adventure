using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class EnemyManageHealthboxes : StateMachineBehaviour
    {
        CharacterManager characterManager;

        public bool activateHealthHitboxes = false;
        public bool deactivateHealthHitboxes = false;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            animator.gameObject.TryGetComponent<CharacterManager>(out characterManager);

            if (characterManager == null)
            {
                characterManager = animator.GetComponentInParent<CharacterManager>(true);
            }

            /*
                        if (activateHealthHitboxes)
                        {
                            characterManager.enemyHealthController.EnableHealthHitboxes();
                        }
                        else if (deactivateHealthHitboxes)
                        {
                            characterManager.enemyHealthController.DisableHealthHitboxes();
                        }*/
        }
    }

}
