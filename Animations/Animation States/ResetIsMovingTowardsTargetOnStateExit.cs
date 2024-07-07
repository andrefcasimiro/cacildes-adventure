using UnityEngine;

namespace AF
{
    public class ResetIsMovingTowardsTargetOnStateExit : StateMachineBehaviour
    {
        CharacterManager character;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (character == null)
            {
                animator.TryGetComponent(out character);
            }

            if (character != null)
            {
                character.isCuttingDistanceToTarget = false;
            }
        }
    }
}
