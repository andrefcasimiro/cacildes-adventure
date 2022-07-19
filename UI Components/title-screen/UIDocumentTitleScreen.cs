using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace AF
{

    public class UIDocumentTitleScreen : UIDocumentBase
    {
        public int startingSceneIndex;

        protected override void Start()
        {
            base.Start();

            Button newGameButton = this.root.Q<Button>("NewGameButton");
            SetupButtonClick(newGameButton, () =>
            {
                this.Disable();
                SceneManager.LoadScene(startingSceneIndex);
            });
            newGameButton.Focus();

            Button loadGameButton = this.root.Q<Button>("LoadGameButton");
            SetupButtonClick(loadGameButton, () =>
            {
                this.Disable();
                SaveSystem.instance.LoadGameData();
            });

            Button exitGameButton = this.root.Q<Button>("ExitGameButton");
            SetupButtonClick(exitGameButton, () =>
            {
                this.Disable();
                Application.Quit();
            });

        }


    }

}