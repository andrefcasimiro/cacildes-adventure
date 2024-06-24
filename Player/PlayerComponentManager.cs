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

        public bool regainControlOnResetState = false;

        // Cache
        int nothingLayer;
        int enemyLayer;

        public bool isParalysed = false;

        private void Start()
        {
            nothingLayer = LayerMask.GetMask("Nothing");
            enemyLayer = LayerMask.GetMask("Enemy");
        }

        public bool PlayerMovementIsEnabled()
        {
            return thirdPersonController.enabled;
        }

        public void ResetStates()
        {
            if (regainControlOnResetState)
            {
                regainControlOnResetState = false;

                EnablePlayerControl();
            }

            EnableCollisionWithEnemies();
        }

        public void EnableComponents()
        {
            if (isParalysed)
            {
                return;
            }

            thirdPersonController.enabled = true;
            thirdPersonController.canRotateCharacter = true;
            thirdPersonController.canMove = true;
            playerCombatController.enabled = true;
            dodgeController.enabled = true;
            playerParryManager.enabled = true;
            thirdPersonController.SetTrackFallDamage(true);
        }

        public void DisableComponents()
        {
            thirdPersonController.StopMovement();
            thirdPersonController.canMove = false;
            thirdPersonController.canRotateCharacter = false;
            playerCombatController.enabled = false;
            dodgeController.enabled = false;
            playerParryManager.enabled = false;
            thirdPersonController.SetTrackFallDamage(false);
        }

        public void DisableCharacterController()
        {
            characterController.enabled = false;
        }

        public void EnableCharacterController()
        {
            characterController.enabled = true;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <returns></returns>
        public void DisablePlayerControl()
        {
            DisableCharacterController();
            DisableComponents();
        }

        public void DisablePlayerControlAndRegainControlAfterResetStates()
        {
            DisableCharacterController();
            DisableComponents();

            regainControlOnResetState = true;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <returns></returns>
        public void EnablePlayerControl()
        {
            EnableCharacterController();
            EnableComponents();
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
            bool originalTrackFallDamage = thirdPersonController.GetTrackFallDamage();

            // Disable fall damage tracking temporarily
            thirdPersonController.SetTrackFallDamage(false);

            // Disable character controller to avoid unintended collisions during position update
            DisableCharacterController();

            characterController.transform.SetPositionAndRotation(newPosition, newRotation);
            EnableCharacterController();

            // Restore original fall damage tracking state
            thirdPersonController.SetTrackFallDamage(originalTrackFallDamage);
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

        public void SetIsParalysed(bool value)
        {
            isParalysed = value;

            if (!value)
            {
                EnablePlayerControl();
            }
            else
            {
                LockPlayerControl();
            }
        }

        public void LockPlayerControl()
        {
            DisableComponents();
            thirdPersonController.enabled = false;
        }

        public void FaceObject(GameObject gameObject)
        {
            Vector3 desiredRot = gameObject.transform.position - characterController.transform.position;
            desiredRot.y = 0;
            characterController.transform.rotation = Quaternion.LookRotation(desiredRot);
        }
    }
}
