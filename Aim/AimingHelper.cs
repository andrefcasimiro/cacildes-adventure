
using UnityEngine;
using UnityEngine.Animations;

namespace AF.Aim
{
    public class AimingHelper : MonoBehaviour
    {
        //public PlayerLocomotionManager playerLocomotionManager;
        //public PlayerRotationManager playerRotationManager;
        public GameObject aimingCamera;

        [Header("Aiming Settigns")]
        public LookAtConstraint lookAtConstraint;

        private void Awake()
        {
            this.enabled = false;
        }

        void Update()
        {
            bool activateAimCamera = ShouldActivateAimCamera();

            if (activateAimCamera)
            {
                /*if (playerRotationManager.rotateWithCamera == false)
                {
                    playerRotationManager.rotateWithCamera = true;
                }*/

                if (aimingCamera.gameObject.activeSelf == false)
                {
                    aimingCamera.SetActive(true);
                }

                if (lookAtConstraint.constraintActive == false)
                {
                    Debug.Log("Hey");
                    lookAtConstraint.constraintActive = true;
                }
            }
            else
            {
                /*if (playerRotationManager.rotateWithCamera == true)
                {
                    playerRotationManager.rotateWithCamera = false;
                }*/

                if (aimingCamera.gameObject.activeSelf == true)
                {
                    aimingCamera.SetActive(false);
                }

                if (lookAtConstraint.constraintActive == true)
                {
                    Debug.Log("How");
                    lookAtConstraint.constraintActive = false;
                }
            }
        }

        bool ShouldActivateAimCamera()
        {
            return true;
            // return playerLocomotionManager.Movement.x == 0 && playerLocomotionManager.Movement.y == 0;
        }

        private void OnDisable()
        {
            aimingCamera.gameObject.SetActive(false);
            // playerRotationManager.rotateWithCamera = false;
            lookAtConstraint.constraintActive = false;
        }
    }

}
