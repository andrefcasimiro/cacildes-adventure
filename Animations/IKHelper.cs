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

        bool CanUseIK()
        {
            if (!equipmentDatabase.isTwoHanding)
            {
                return false;
            }

            if (!playerManager.canUseWeaponIK)
            {
                return false;
            }

            if (playerManager.dodgeController.isDodging)
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
