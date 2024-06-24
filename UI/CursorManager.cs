using AF.Shops;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF
{
    public class CursorManager : MonoBehaviour
    {
        public GameSession gameSession;

        [Header("UI Documents")]
        public ErrorHandler errorHandler;
        public UIDocumentCharacterCustomization uIDocumentCharacterCustomization;

        UIDocumentTitleScreen _uIDocumentTitleScreen;

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

            if (uIDocumentCharacterCustomization.isActiveAndEnabled)
            {
                return false;
            }


            if (IsTitleScreenActive())
            {
                return false;
            }

            return true;
        }

        bool IsTitleScreenActive()
        {
            if (_uIDocumentTitleScreen == null)
            {
                _uIDocumentTitleScreen = FindAnyObjectByType<UIDocumentTitleScreen>(FindObjectsInactive.Include);
            }

            return _uIDocumentTitleScreen != null && _uIDocumentTitleScreen.isActiveAndEnabled;
        }
    }
}
