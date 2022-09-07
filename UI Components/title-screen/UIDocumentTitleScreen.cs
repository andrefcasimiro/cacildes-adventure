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

        //References
        Button newGameButton;
        Button loadGameButton;
        Button controlsButton;
        Button exitGameButton;

        protected override void Start()
        {
            base.Start();

            StartCoroutine(FadeAndShowTitleScreen());
        }

        IEnumerator FadeAndShowTitleScreen()
        {
            yield return null;

             newGameButton = this.root.Q<Button>("NewGameButton");
            SetupButtonClick(newGameButton, () =>
            {
                StartCoroutine(BeginGame());
            });
            newGameButton.Focus();

             loadGameButton = this.root.Q<Button>("LoadGameButton");
            SetupButtonClick(loadGameButton, () =>
            {
                this.Disable();
                SaveSystem.instance.LoadGameData();
            });


             controlsButton = this.root.Q<Button>("ControlsGameButton");
            SetupButtonClick(controlsButton, () =>
            {
                this.Disable();

                FindObjectOfType<UIDocumentControlsScreen>(true).Enable();
            });


             exitGameButton = this.root.Q<Button>("ExitGameButton");
            SetupButtonClick(exitGameButton, () =>
            {
                this.Disable();
                Application.Quit();
            });

            Button itchIoButton = this.root.Q<Button>("ItchIo");
            SetupButtonClick(itchIoButton, () =>
            {
                Application.OpenURL("https://twitter.com/CacildesGame");
            });

            Button twitterButton = this.root.Q<Button>("Twitter");
            SetupButtonClick(twitterButton, () =>
            {
                Application.OpenURL("https://twitter.com/CacildesGame");
            });

            EV_FadeIn evFadeIn = FindObjectOfType<EV_FadeIn>(true);
            if (evFadeIn != null)
            {
                yield return evFadeIn.Dispatch();
            }
        }

        IEnumerator BeginGame()
        {
            EV_FadeOut evFadeOut = FindObjectOfType<EV_FadeOut>(true);
            if (evFadeOut != null)
            {
                yield return evFadeOut.Dispatch();
            }

            this.Disable();
            SceneManager.LoadScene(startingSceneIndex);
        }


    }

}