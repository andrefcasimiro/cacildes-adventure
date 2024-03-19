using UnityEngine;

namespace AF
{
    public class CursorManager : MonoBehaviour
    {
        public GameSession gameSession;

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
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
