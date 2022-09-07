using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace AF
{

    public class UIDocumentGameOverScreen : UIDocumentBase
    {
        Button loadPreviousGameBtn;
        Button exitToMainMenuBtn;

        public AudioClip gameOverFanfare;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            this.loadPreviousGameBtn = this.root.Q<Button>("btnLoadPreviousSave");

            SetupButtonClick(this.loadPreviousGameBtn, () =>
            {
                this.Disable();
                SaveSystem.instance.LoadGameData();
            });

            this.exitToMainMenuBtn = this.root.Q<Button>("btnExitToMainMenu");

            SetupButtonClick(this.exitToMainMenuBtn, () =>
            {
                this.Disable();
                SceneManager.LoadScene("00 - Title Screen");
            });

            this.Disable();
        }

        public void ShowGameOverScreen()
        {
            BGMManager.instance.StopMusic();
            BGMManager.instance.PlaySound(gameOverFanfare, null);

            this.Enable();

            EventPage[] eventPages = FindObjectsOfType<EventPage>(true);
            foreach (EventPage evPage in eventPages)
            {
                if (evPage.isRunning)
                {
                    evPage.StopAllCoroutines();
                    evPage.isRunning = false;
                }
            }

            UIDocumentDialogue uIDocumentDialogue = FindObjectOfType<UIDocumentDialogue>(true);
            uIDocumentDialogue.Disable();

        }
    }
}
