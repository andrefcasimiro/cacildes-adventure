using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentKeyPressPromptUI : UIDocumentBase
    {
        Label keyPressPromptText;

        protected override void Start()
        {
            base.Start();

            this.keyPressPromptText = this.root.Q<Label>("Text");

            this.Disable();
        }

        public void Show(string message)
        {
            this.keyPressPromptText.text = message;

            this.Enable();
        }

    }

}
