using UnityEngine;
using UnityEngine.InputSystem;

namespace AF
{

    public static class Utils
    {

        public static void FaceTarget(Transform origin, Transform target)
        {
            var lookPos = target.position - origin.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);

            origin.rotation = rotation;
        }

        public static void ShowCursor()
        {
            if (Gamepad.current == null)
            {
                Cursor.visible = true;
            }
            else
            {
                Cursor.visible = false;
            }

            Cursor.lockState = CursorLockMode.None;
        }

        public static void HideCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }

}
