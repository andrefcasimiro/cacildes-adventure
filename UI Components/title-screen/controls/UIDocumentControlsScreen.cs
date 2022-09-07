using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace AF
{

    public class UIDocumentControlsScreen : UIDocumentBase
    {
        protected override void Start()
        {
            base.Start();

            Button goGackButton = this.root.Q<Button>("GoBackButton");
            SetupButtonClick(goGackButton, () =>
            {
                this.Disable();

                FindObjectOfType<UIDocumentTitleScreen>(true).Enable();
            });
            goGackButton.Focus();


            this.Disable();
        }


    }

}