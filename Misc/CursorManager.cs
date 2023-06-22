using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AF
{
    public class CursorManager : MonoBehaviour
    {
        GamepadCursor gamepadCursor;

        private void Awake()
        {
            gamepadCursor = FindObjectOfType<GamepadCursor>(true);
        }

        public void ShowCursor()
        {
            if (Gamepad.current != null)
            {
                Cursor.visible = false;

                gamepadCursor.gameObject.SetActive(true);
            }
            else
            {
                Cursor.visible = true;
            }

            Cursor.lockState = CursorLockMode.None;
        }

        public void HideCursor()
        {
            if (Gamepad.current != null)
            {
                gamepadCursor.gameObject.SetActive(false);
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }
}