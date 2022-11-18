using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace AF
{

    public class UIDocumentGameOver : MonoBehaviour
    {

        private void Awake()
        {
            this.gameObject.SetActive(false);

        }

        private void Update()
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }

        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            BGMManager.instance.StopMusic();
            BGMManager.instance.PlayGameOver();

            EventPage[] eventPages = FindObjectsOfType<EventPage>(true);
            foreach (EventPage evPage in eventPages)
            {
                if (evPage.IsRunning())
                {
                    evPage.Stop();
                }
            }

            FindObjectOfType<PlayerComponentManager>(true).DisableCharacterController();
            FindObjectOfType<PlayerComponentManager>(true).DisableComponents();

            FindObjectOfType<UIDocumentDialogueWindow>(true).gameObject.SetActive(false);

            var menuManager = FindObjectOfType<MenuManager>(true);

            FindObjectOfType<GamepadCursor>(true).gameObject.SetActive(true);

            menuManager.SetupButton(root.Q<Button>("btnLoadLastSave"), () =>
            {

                FindObjectOfType<GamepadCursor>(true).gameObject.SetActive(false);
                this.gameObject.SetActive(false);
                SaveSystem.instance.LoadLastSavedGame();
            });
            menuManager.SetupButton(root.Q<Button>("btnExitGame"), () =>
            {
                Application.Quit();
            });


            FindObjectOfType<GamepadCursor>(true).gameObject.SetActive(true);
        }

    }
}