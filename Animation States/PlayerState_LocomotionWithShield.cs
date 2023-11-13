using UnityEngine;

namespace AF
{
    public class PlayerState_LocomotionWithShield : StateMachineBehaviour
    {
        AudioSource playerAudioSource;
        PlayerManager playerManager;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (playerManager == null)
            {
                animator.TryGetComponent(out playerManager);
            }

            if (playerAudioSource == null)
            {
                playerAudioSource = playerManager.equipmentGraphicsHandler.GetComponent<PlayerCombatController>().combatAudioSource;
            }

            playerManager.equipmentGraphicsHandler.HideWeapons();
            playerManager.equipmentGraphicsHandler.ShowShield();

            if (Player.instance.equippedShield != null && Player.instance.equippedShield.shieldDrawSfx != null)
            {
                playerAudioSource.PlayOneShot(Player.instance.equippedShield.shieldDrawSfx);
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator.GetBool("IsBlocking") == false)
            {
                playerManager.equipmentGraphicsHandler.HideShield();
            }
            else
            {
                playerManager.equipmentGraphicsHandler.ShowShield();
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            playerManager.equipmentGraphicsHandler.ShowWeapons();

            if (animator.GetBool("IsBlocking") == false)
            {
                playerManager.equipmentGraphicsHandler.HideShield();
            }
        }
    }
}
