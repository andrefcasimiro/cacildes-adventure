using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

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
            // UnityEngine.Cursor.lockState = CursorLockMode.None;
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

            LostCoinsManager.instance.SetCoinsToRecover(FindObjectOfType<PlayerCombatController>(true).transform);


            StartCoroutine(Reload());

        }

        IEnumerator Reload()
        {
            yield return new WaitForSeconds(4f);
            SaveSystem.instance.loadingFromGameOver = true;
            SaveSystem.instance.LoadLastSavedGame();
        }

    }
}