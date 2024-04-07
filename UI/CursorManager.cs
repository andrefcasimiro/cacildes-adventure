using UnityEngine;

namespace AF
{
    public class CursorManager : MonoBehaviour
    {
        public GameSession gameSession;

        public ErrorHandler errorHandler;

        private void Start()
        {
            if (gameSession.gameState == GameSession.GameState.INITIALIZED_AND_SHOWN_TITLE_SCREEN)
            {
                Invoke(nameof(HideCursor), 1f);
            }
        }

        public void ShowCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void HideCursor()
        {
            if (CanHideCursor())
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        public bool IsVisible()
        {
            return Cursor.visible == true;
        }

        public bool CanHideCursor()
        {
            if (errorHandler.HasErrors())
            {
                return false;
            }

            return true;
        }
    }
}
