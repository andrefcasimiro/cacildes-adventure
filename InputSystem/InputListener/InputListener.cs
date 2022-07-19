using UnityEngine;

namespace AF
{

    public class InputListener : MonoBehaviour
    {
        [HideInInspector]
        public bool hasPressedConfirmButton;
        [HideInInspector]
        public bool hasPressedCancelButton;

        protected InputActions inputActions;

        protected void OnEnable()
        {
            inputActions = new InputActions();

            // Decision Button Input
            inputActions.PlayerActions.Confirm.performed += ctx =>
            {
                hasPressedConfirmButton = true;
            };

            inputActions.PlayerActions.Confirm.canceled += ctx =>
            {
                hasPressedConfirmButton = false;
            };

            // Cancel Button Input
            inputActions.PlayerActions.MainMenu.performed += ctx =>
            {
                hasPressedCancelButton = true;
            };

            inputActions.PlayerActions.MainMenu.canceled += ctx =>
            {
                hasPressedCancelButton = false;
            };

            inputActions.Enable();

        }


        protected void OnDisable()
        {
            if (inputActions != null)
            {
                inputActions.Disable();
                inputActions = null;
            }
        }

    }
}

