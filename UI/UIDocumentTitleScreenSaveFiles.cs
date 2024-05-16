using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentTitleScreenSaveFiles : MonoBehaviour
    {
        VisualElement root;

        public VisualTreeAsset saveFileButtonPrefab;
        ScrollView scrollPanel;

        [Header("Components")]
        public UIManager uiManager;
        public Soundbank soundbank;
        public SaveManager saveManager;

        [Header("UI Components")]
        public UIDocumentTitleScreen uIDocumentTitleScreen;

        private void Awake()
        {
            gameObject.SetActive(false);
        }


        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnClose()
        {
            if (this.isActiveAndEnabled)
            {
                Close();
            }
        }

        private void OnEnable()
        {
            root = GetComponent<UIDocument>().rootVisualElement;

            scrollPanel = root.Q<ScrollView>("SaveFilesContainer");

            DrawUI();
        }

        void DrawUI()
        {
            scrollPanel.Clear();

            Button exitButton = new()
            {
                text = "Return to Title Screen"
            };
            exitButton.AddToClassList("primary-button");
            scrollPanel.Add(exitButton);

            UIUtils.SetupButton(exitButton, () =>
            {
                Close();
            }, soundbank);

            exitButton.RegisterCallback<FocusInEvent>((ev) =>
            {
                scrollPanel.ScrollTo(exitButton);
            });

            exitButton.Focus();



            Button openSavesFolder = new()
            {
                text = "Open Saves Folder"
            };
            openSavesFolder.AddToClassList("primary-button");

            UIUtils.SetupButton(openSavesFolder, () =>
            {
                // Open the folder using the default file explorer
                Process.Start(Application.persistentDataPath + "/" + saveManager.SAVE_FILES_FOLDER);
            }, soundbank);

            scrollPanel.Add(openSavesFolder);


            foreach (var saveFileName in SaveUtils.GetSaveFileNames(saveManager.SAVE_FILES_FOLDER))
            {
                var saveFileInstance = saveFileButtonPrefab.CloneTree();

                saveFileInstance.Q<Label>("SaveFileName").text = saveFileName;

                Texture2D screenshotThumbnail = SaveUtils.GetScreenshotFilePath(saveManager.SAVE_FILES_FOLDER, saveFileName);
                saveFileInstance.Q<VisualElement>("SaveScreenshot").style.backgroundImage = screenshotThumbnail;
                saveFileInstance.Q<VisualElement>("SaveScreenshot").Q<Label>("SaveFileNotFoundLabel").style.display =
                    screenshotThumbnail == null ? DisplayStyle.Flex : DisplayStyle.None;

                UIUtils.SetupButton(saveFileInstance.Q<Button>("Button"), () =>
                {
                    saveManager.LoadSaveFile(saveFileName);
                }, soundbank);

                scrollPanel.Add(saveFileInstance);
            }
        }

        void Close()
        {
            uIDocumentTitleScreen.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

    }
}