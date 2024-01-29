using UnityEngine;
using UnityEngine.InputSystem;

namespace AF
{
    public class CursorManager : MonoBehaviour
    {
        private void Awake()
        {
            // Hide Cursor By Default On Every Scene
            HideCursor();
        }

        public void ShowCursor()
        {
            if (Gamepad.current != null)
            {
                Cursor.visible = false;
            }
            else
            {
                Cursor.visible = true;
            }

            Cursor.lockState = CursorLockMode.None;
        }

        public void HideCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
