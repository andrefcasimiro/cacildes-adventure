using System.Collections;
using UnityEngine;
using StarterAssets;

namespace AF
{

    public class PlayerComponentManager : MonoBehaviour
    {
        ThirdPersonController thirdPersonController => GetComponent<ThirdPersonController>();
        PlayerCombatController playerCombatController => GetComponent<PlayerCombatController>();
        DodgeController dodgeController => GetComponent<DodgeController>();
        PlayerParryManager playerParryManager => GetComponent<PlayerParryManager>();

        public bool isInTutorial = false;
        public bool isInBonfire = false;

        CharacterController characterController => GetComponent<CharacterController>();

        private void Start()
        {
        }

        public void EnableComponents()
        {
            thirdPersonController.enabled = true;
            playerCombatController.enabled = true;
            dodgeController.enabled = true;
            playerParryManager.enabled = true;
        } 

        public void DisableComponents()
        {
            thirdPersonController.StopMovement();
            thirdPersonController.enabled = false;
            playerCombatController.enabled = false;
            dodgeController.enabled = false;
            playerParryManager.enabled = false;
        }

        public void DisableCharacterController()
        {
            characterController.enabled = false;
        }

        public void EnableCharacterController()
        {
            characterController.enabled = true;
        }

        public bool IsBusy()
        {
            if (isInTutorial)
            {
                return true;
            }

            if (isInBonfire)
            {
                return true;
            }

            return false;
        }

        public void CurePlayer()
        {
            GetComponent<HealthStatManager>().RestoreHealthPercentage(100);
            GetComponent<StaminaStatManager>().RestoreStaminaPercentage(100);
            GetComponent<PlayerStatusManager>().RemoveAllStatus();
        }

    }

}