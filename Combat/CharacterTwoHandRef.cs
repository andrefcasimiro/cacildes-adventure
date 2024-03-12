using UnityEngine;
namespace AF
{

    public class CharacterTwoHandRef : MonoBehaviour
    {

        [Header("Player Settings")]
        public bool useTwoHandingTransform = true;
        public Vector3 twoHandingPosition;
        public Vector3 twoHandingRotation;
        Vector3 originalPosition;
        Quaternion originalRotation;

        [Header("Dual Wielding")]
        public PlayerManager playerManager;
        public CharacterWeaponHitbox leftWeapon;
        public EquipmentDatabase equipmentDatabase;

        private void Awake()
        {
            this.originalPosition = transform.localPosition;
            this.originalRotation = transform.localRotation;

            if (useTwoHandingTransform && equipmentDatabase.isTwoHanding)
            {
                UseTwoHandTransform();
            }
            else
            {
                UseDefaultTransform();
            }
        }

        public void UseDefaultTransform()
        {
            this.transform.localPosition = originalPosition;
            this.transform.localRotation = originalRotation;

            if (leftWeapon != null)
            {
                playerManager.playerWeaponsManager.UnequipLeftWeapon();
            }
        }

        public void UseTwoHandTransform()
        {
            if (leftWeapon != null)
            {
                playerManager.playerWeaponsManager.EquipLeftWeapon(leftWeapon);
            }

            if (useTwoHandingTransform == false)
            {
                return;
            }

            this.transform.localPosition = twoHandingPosition;
            this.transform.localEulerAngles = twoHandingRotation;
        }

        private void OnDisable()
        {
            if (leftWeapon != null)
            {
                playerManager.playerWeaponsManager.UnequipLeftWeapon();
            }
        }

    }
}
