using UnityEngine;
using UnityEngine.UI;

namespace AF
{

    public class KeyPressPromptManager : InputListener
    {
        UIDocumentKeyPressPromptUI uiDocumentKeyPressPromptUI => GetComponent<UIDocumentKeyPressPromptUI>();

        private void Update()
        {
            if (hasPressedConfirmButton)
            {
                if (uiDocumentKeyPressPromptUI.IsVisible())
                {
                    uiDocumentKeyPressPromptUI.Disable();
                }
            }
        }

        public void ShowNotification(string message)
        {
            uiDocumentKeyPressPromptUI.Show(message);
        }

        public void Close()
        {
            uiDocumentKeyPressPromptUI.Disable();
        }
    }

}
