using System.Collections;
using UnityEngine;

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

        // Cache
        int nothingLayer;
        int enemyLayer;

        private void Start()
        {
            nothingLayer = LayerMask.GetMask("Nothing");
            enemyLayer = LayerMask.GetMask("Enemy");
        }

        public bool PlayerMovementIsEnabled()
        {
            return thirdPersonController.enabled;
        }

        public void EnableComponents()
        {
            thirdPersonController.enabled = true;
            thirdPersonController.canMove = true;
            playerCombatController.enabled = true;
            dodgeController.enabled = true;
            playerParryManager.enabled = true;
        }

        public void DisableComponents()
        {
            thirdPersonController.StopMovement();
            thirdPersonController.canMove = false;
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

        public void UpdatePosition(Vector3 newPosition, Quaternion newRotation)
        {
            // Store the initial state of fall damage tracking
            bool originalTrackFallDamage = thirdPersonController.trackFallDamage;

            // Disable fall damage tracking temporarily
            thirdPersonController.trackFallDamage = false;

            // Disable character controller to avoid unintended collisions during position update
            DisableCharacterController();

            transform.position = newPosition;
            if (newRotation != Quaternion.identity)
            {
                transform.rotation = newRotation;
            }

            EnableCharacterController();

            // Restore original fall damage tracking state
            thirdPersonController.trackFallDamage = originalTrackFallDamage;

            thirdPersonController.fallDamageInitialized = true;
        }

        public void EnableCollisionWithEnemies()
        {
            if (characterController.excludeLayers != nothingLayer)
            {
                characterController.excludeLayers = nothingLayer;
            }
        }

        public void DisableCollisionWithEnemies()
        {
            characterController.excludeLayers = enemyLayer;
        }

        public void TeleportPlayer(Transform target)
        {
            UpdatePosition(target.position, target.rotation);
        }
    }

}
