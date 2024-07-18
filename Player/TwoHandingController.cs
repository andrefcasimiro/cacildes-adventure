using AF.Events;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{

    public class TwoHandingController : MonoBehaviour
    {
        public PlayerManager playerManager;
        public Soundbank soundbank;

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;

        public UnityAction onTwoHandingModeChanged;

        private void Awake()
        {
            UpdateTwoHandingMode();

            EventManager.StartListening(EventMessages.ON_TWO_HANDING_CHANGED, UpdateTwoHandingMode);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnInput()
        {
            if (playerManager.climbController.climbState != Ladders.ClimbState.NONE)
            {
                return;
            }

            if (playerManager.thirdPersonController.isSwimming)
            {
                return;
            }

            equipmentDatabase.SetIsTwoHanding(!equipmentDatabase.isTwoHanding);
            soundbank.PlaySound(soundbank.switchTwoHand);
        }

        public void UpdateTwoHandingMode()
        {
            playerManager.UpdateAnimatorOverrideControllerClips();

            onTwoHandingModeChanged?.Invoke();
        }
    }
}
