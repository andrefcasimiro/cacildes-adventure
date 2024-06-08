using UnityEngine;

namespace AF
{
    public class RotateTowardsTargetOnStateEnter : StateMachineBehaviour
    {
        CharacterManager character;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (character == null)
            {
                animator.TryGetComponent(out character);
            }

            if (character != null)
            {
                character.FaceTarget();
            }
        }
    }
}
