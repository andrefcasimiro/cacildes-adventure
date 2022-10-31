using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{

    public class UIDocumentSaveMenu : MonoBehaviour
    {
        MenuManager menuManager;
        VisualElement root;

        public VisualTreeAsset saveButtonEntryPrefab;

        float savedGameCooldownTimer = Mathf.Infinity;
        float maxSavedGameCoooldownTimer = 1f;
        bool hasFinishedTimer = true;

        private void Awake()
        {
            menuManager = FindObjectOfType<MenuManager>(true);
        }

        private void OnEnable()
        {
            this.root = GetComponent<UIDocument>().rootVisualElement;
            menuManager.SetupNavMenu(root);

            DrawUI();
        }

        private void Update()
        {
            if (savedGameCooldownTimer < maxSavedGameCoooldownTimer)
            {
                savedGameCooldownTimer += Time.deltaTime;
            }
            else if (hasFinishedTimer == false)
            {
                DrawUI();
                hasFinishedTimer = true;
            }
        }

        void DrawUI()
        {
            menuManager.SetActiveMenu(root, "ButtonSave");

            root.Q<Button>("SaveGameButton").RegisterCallback<ClickEvent>(ev =>
            {
                SaveSystem.instance.SaveGameData(SaveSystem.instance.SAVE_FILE_NAME + System.DateTime.Now.ToFileTimeUtc());
                hasFinishedTimer = false;
                savedGameCooldownTimer = 0f;
                DrawUI();
            });

            List<SaveFileEntry> saveFiles = SaveSystem.instance.GetSaveFiles();

            bool saveIsAllowed = saveFiles.Count <= SaveSystem.instance.MAX_SAVE_FILES_ALLOWED;

            root.Q<Button>("SaveGameButton").SetEnabled(saveIsAllowed || maxSavedGameCoooldownTimer < savedGameCooldownTimer);

            root.Q<Label>("MaxLimitWarn").style.opacity = saveIsAllowed ? 0 : 1;

            root.Q<ScrollView>().Clear();
            root.Q<VisualElement>("ItemPreview").style.opacity = 0;

            foreach (var saveFileEntry in saveFiles)
            {
                var btn = saveButtonEntryPrefab.CloneTree();

                btn.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(saveFileEntry.screenshot);

                btn.Q<Label>("Name").text = (saveFileEntry.isQuickSave ? "Quick Save / " : "") + saveFileEntry.sceneName + " / Lv " + saveFileEntry.level;
                btn.Q<Label>("Value").text = System.DateTime.Parse(saveFileEntry.creationDate).ToString();
                btn.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(saveFileEntry.screenshot);

                // btn.Q<Button>("DeleteButton").style.display = saveFileEntry.fileFullPath.ToLower().Contains("quicksave") ? DisplayStyle.None : DisplayStyle.Flex;

                btn.Q<Button>("DeleteButton").RegisterCallback<ClickEvent>(ev =>
                {
                    SaveSystem.instance.DeleteSaveFile(saveFileEntry.fileFullPath, saveFileEntry.fileFullPath.Replace("json", "jpg"));

                    DrawUI();
                });

                btn.RegisterCallback<MouseEnterEvent>(ev =>
                {
                    root.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(saveFileEntry.screenshot);
                    root.Q<Label>("Title").text = saveFileEntry.sceneName + ", " + saveFileEntry.level;
                    root.Q<Label>("SubTitle").text = System.DateTime.Parse(saveFileEntry.creationDate).ToString();

                    var totalGameTime = System.TimeSpan.FromSeconds(saveFileEntry.gameTime);
                    root.Q<Label>("Description").text = "Total Game Time: " + totalGameTime.Hours + "h:" + totalGameTime.Minutes + "m:" + totalGameTime.Seconds + "s";

                    root.Q<VisualElement>("ItemPreview").style.opacity = 1;
                });

                btn.RegisterCallback<MouseLeaveEvent>(ev =>
                {
                    root.Q<VisualElement>("ItemPreview").style.opacity = 0;
                });

                root.Q<ScrollView>().Add(btn);
            }

        }

    }

}