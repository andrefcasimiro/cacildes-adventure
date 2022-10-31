using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{

    public class UIDocumentTitleScreen : MonoBehaviour
    {
        UIDocument uIDocument => GetComponent<UIDocument>();

        TitleScreenManager titleScreenManager => GetComponentInParent<TitleScreenManager>();


        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            FindObjectOfType<UIDocumentPlayerHUDV2>(true).gameObject.SetActive(false);

            root.Q<Button>("NewGameButton").RegisterCallback<ClickEvent>(ev =>
            {
                FindObjectOfType<UIDocumentPlayerHUDV2>(true).gameObject.SetActive(true);

                titleScreenManager.StartGame();

                this.gameObject.SetActive(false);
            });
            root.Q<Button>("ContinueButton").RegisterCallback<ClickEvent>(ev =>
            {
                FindObjectOfType<UIDocumentTitleScreenLoadMenu>(true).gameObject.SetActive(true);
                this.gameObject.SetActive(false);

            });
            root.Q<Button>("CreditsButton").RegisterCallback<ClickEvent>(ev =>
            {
                FindObjectOfType<UIDocumentTitleScreenCredits>(true).gameObject.SetActive(true);
                this.gameObject.SetActive(false);
            });

        }

        private void Update()
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }

    }
}
