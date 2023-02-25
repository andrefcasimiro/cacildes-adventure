using System.Collections;
using UnityEngine;

namespace AF
{
    public class UIDocumentGameOver : MonoBehaviour
    {
        private void Awake()
        {
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            BGMManager.instance.StopMusic();
            Soundbank.instance.PlayGameOver();

            EventPage[] eventPages = FindObjectsOfType<EventPage>(true);
            foreach (EventPage evPage in eventPages)
            {
                if (evPage.isRunning)
                {
                    evPage.isRunning = false;
                    evPage.StopAllCoroutines();
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
