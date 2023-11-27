using UnityEngine;

namespace AF
{
    public class ResetCharacterStatesOnStateEnter : StateMachineBehaviour
    {
        CharacterBaseManager character;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (character == null)
            {
                animator.TryGetComponent(out character);
            }

            character.ResetStates();
        }
    }
}
