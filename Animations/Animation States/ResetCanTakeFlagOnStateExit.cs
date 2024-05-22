using UnityEngine;

namespace AF
{
    public class ResetCanTakeFlagOnStateExit : StateMachineBehaviour
    {
        CharacterBaseManager character;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (character == null)
            {
                animator.TryGetComponent(out character);
            }

            if (character.damageReceiver != null)
            {
                character.damageReceiver.SetCanTakeDamage(true);
            }
        }
    }
}
