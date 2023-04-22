using UnityEngine;

namespace AF {
    public class PlayerState_LocomotionWithShield : StateMachineBehaviour
    {
        EquipmentGraphicsHandler equipmentGraphicsHandler;
        AudioSource playerAudioSource;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (equipmentGraphicsHandler == null)
            {
                equipmentGraphicsHandler = animator.GetComponent<EquipmentGraphicsHandler>();
            }
            if (playerAudioSource == null)
            {
                playerAudioSource = equipmentGraphicsHandler.GetComponent<PlayerCombatController>().combatAudioSource;
            }

            equipmentGraphicsHandler.HideWeapons();
            equipmentGraphicsHandler.ShowShield();

            if (Player.instance.equippedShield != null && Player.instance.equippedShield.shieldDrawSfx != null)
            {
                playerAudioSource.PlayOneShot(Player.instance.equippedShield.shieldDrawSfx);
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator.GetBool("IsBlocking") == false)
            {
                equipmentGraphicsHandler.HideShield();
            }else
            {
                equipmentGraphicsHandler.ShowShield();
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            equipmentGraphicsHandler.ShowWeapons();
         
            if (animator.GetBool("IsBlocking") == false)
            {
                equipmentGraphicsHandler.HideShield();
            }
        }
    }
}
