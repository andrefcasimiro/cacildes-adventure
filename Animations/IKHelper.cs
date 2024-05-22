using System.Collections;
using AF.Events;
using TigerForge;
using UnityEngine;

namespace AF
{
    public class IKHelper : MonoBehaviour
    {

        public PlayerManager playerManager;
        public EquipmentDatabase equipmentDatabase;
        public Transform playerHand; // Reference to the player's hand transform

        [Range(0f, 1f)]
        public float weight = 1f; // Weight parameter controlling the influence of the IK effect
        float initialWeight;

        public float resetDuration = 0.1f;

        [Header("Options")]
        public bool dontAllowIKWhenRunning = false;
        public bool dontAllowIKWhenJumpAttacking = false;
        public bool onlyAllowOnAttacks = false;

        private void Awake()
        {
            initialWeight = weight;

            EventManager.StartListening(EventMessages.ON_CAN_USE_IK_IS_TRUE, OnResetCanUseIK);
        }

        void OnResetCanUseIK()
        {
            if (!this.isActiveAndEnabled)
            {
                return;
            }

            if (weight < initialWeight)
            {
                return;
            }

            if (!CanUseIK())
            {
                return;
            }

            weight = 0f;

            StartCoroutine(ResetWeightGradually());
        }

        IEnumerator ResetWeightGradually()
        {
            float elapsedTime = 0f;
            while (weight < initialWeight)
            {
                // Calculate the progress based on time elapsed
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / resetDuration;

                // Update the weight based on the progress
                weight = Mathf.Lerp(0f, initialWeight, progress);

                yield return null;
            }

            // Ensure weight reaches exactly initialWeight
            weight = initialWeight;
        }

        bool CanUseIK()
        {
            if (!equipmentDatabase.isTwoHanding)
            {
                return false;
            }

            if (!playerManager.CanUseIK())
            {
                return false;
            }

            if (playerManager.dodgeController.isDodging)
            {
                return false;
            }

            if (playerManager.thirdPersonController.IsSprinting())
            {
                return false;
            }

            if (!playerManager.thirdPersonController.Grounded)
            {
                return false;
            }

            if (playerManager.lockOnManager.isLockedOn)
            {
                return false;
            }

            if (playerManager.playerBlockController.isBlocking)
            {
                return false;
            }

            if (onlyAllowOnAttacks)
            {
                return playerManager.playerCombatController.IsAttacking();
            }

            if (dontAllowIKWhenJumpAttacking && playerManager.playerCombatController.isJumpAttacking)
            {
                return false;
            }

            if (dontAllowIKWhenRunning)
            {
                return playerManager.animator.GetFloat("Speed") <= 1 || playerManager.isBusy;
            }

            return true;
        }

        private void LateUpdate()
        {
            if (!CanUseIK())
            {
                return;
            }
            // Check if the player hand and target transform are assigned
            if (playerHand != null)
            {
                // Calculate blended position and rotation
                Vector3 blendedPosition = Vector3.Lerp(playerHand.position, transform.position, weight);
                Quaternion blendedRotation = Quaternion.Lerp(playerHand.rotation, transform.rotation, weight);

                // Set the player hand position and rotation with the blend
                playerHand.position = blendedPosition;
                playerHand.rotation = blendedRotation;
            }
            else
            {
                Debug.LogWarning("Player hand or target transform is not assigned in IK Helper.");
            }
        }
    }
}
