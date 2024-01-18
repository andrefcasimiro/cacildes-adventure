using System.Collections;
using UnityEngine;

namespace AF
{

    public class PlayerComponentManager : MonoBehaviour
    {
        [Header("Components")]
        public ThirdPersonController thirdPersonController;
        public PlayerCombatController playerCombatController;
        public DodgeController dodgeController;
        public PlayerBlockInput playerParryManager;
        public CharacterController characterController;


        public bool isInTutorial = false;
        public bool isInBonfire = false;


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
            thirdPersonController.canRotateCharacter = true;
            thirdPersonController.canMove = true;
            playerCombatController.enabled = true;
            dodgeController.enabled = true;
            playerParryManager.enabled = true;
        }

        public void DisableComponents()
        {
            thirdPersonController.StopMovement();
            thirdPersonController.canMove = false;
            thirdPersonController.canRotateCharacter = false;
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
            GetComponent<PlayerHealth>().RestoreHealthPercentage(100);
            GetComponent<StaminaStatManager>().RestoreStaminaPercentage(100);
        }

        public void UpdatePosition(Vector3 newPosition, Quaternion newRotation)
        {
            // Store the initial state of fall damage tracking
            bool originalTrackFallDamage = thirdPersonController.trackFallDamage;

            // Disable fall damage tracking temporarily
            thirdPersonController.trackFallDamage = false;

            // Disable character controller to avoid unintended collisions during position update
            DisableCharacterController();

            characterController.transform.SetPositionAndRotation(newPosition, newRotation);
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
            UpdatePosition(target.TransformPoint(Vector3.zero), target.rotation);
        }
    }

}
