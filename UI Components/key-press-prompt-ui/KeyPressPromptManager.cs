using UnityEngine;
using UnityEngine.UI;

namespace AF
{

    public class KeyPressPromptManager : InputListener, ISaveable
    {
        UIDocumentKeyPressPromptUI uiDocumentKeyPressPromptUI => GetComponent<UIDocumentKeyPressPromptUI>();

        private void Start()
        {
            this.Close();
        }

        public void ShowNotification(string message)
        {
            uiDocumentKeyPressPromptUI.Show(message);
        }

        public bool IsVisible()
        {
            return uiDocumentKeyPressPromptUI.IsVisible();
        }

        public void Close()
        {
            uiDocumentKeyPressPromptUI.Disable();
        }

        public void OnGameLoaded(GameData gameData)
        {
            this.Close();
        }
    }

}
