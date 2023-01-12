using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{

    public class UIDocumentLoadMenu : MonoBehaviour
    {
        MenuManager menuManager;
        
        public VisualTreeAsset loadButtonEntryPrefab;

        private void Awake()
        {
            menuManager = FindObjectOfType<MenuManager>(true);
        }

        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            menuManager.SetupNavMenu(root);
            menuManager.SetActiveMenu(root, "ButtonLoad");

            DrawUI(root, true);
        }

        public void DrawUI(VisualElement targetRoot, bool showDeleteButton)
        {

            List<SaveFileEntry> saveFiles = SaveSystem.instance.GetSaveFiles();

            if (saveFiles.Count > 0)
            {
                saveFiles.Sort((a, b) =>
                {
                    var aDate = DateTime.Parse(a.creationDate);
                    var bDate = DateTime.Parse(b.creationDate);

                    return bDate.CompareTo(aDate);
                });

            }

            targetRoot.Q<ScrollView>().Clear();
            targetRoot.Q<VisualElement>("SaveGameItemPreview").style.opacity = 0;

            menuManager = FindObjectOfType<MenuManager>(true);

            foreach (var saveFileEntry in saveFiles)
            {
                var btn = loadButtonEntryPrefab.CloneTree();

                btn.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(saveFileEntry.screenshot);

                btn.Q<Label>("Name").text = saveFileEntry.sceneName + " / Level " + saveFileEntry.level + "";
                btn.Q<Label>("Value").text = System.DateTime.Parse(saveFileEntry.creationDate).ToString();
                btn.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(saveFileEntry.screenshot);

                var loadBtn = btn.Q<Button>("LoadButton");
                var deleteBtn = btn.Q<Button>("DeleteButton");

                deleteBtn.style.display = showDeleteButton ? DisplayStyle.Flex : DisplayStyle.None;

                menuManager.SetupButton(loadBtn, () =>
                {
                    SaveSystem.instance.LoadGameData(saveFileEntry.fileFullPath);
                    menuManager.CloseMenu();
                });
                menuManager.SetupButton(deleteBtn, () =>
                {
                    SaveSystem.instance.DeleteSaveFile(saveFileEntry.fileFullPath, saveFileEntry.fileFullPath.Replace("json", "jpg"));
                    DrawUI(targetRoot, showDeleteButton);
                });

                btn.Q<Button>("LoadButton").text = "Load";

                // Gamepad
                btn.RegisterCallback<FocusInEvent>(ev =>
                {
                    ShowLoadInfo(targetRoot, saveFileEntry);
                });
                btn.RegisterCallback<FocusOutEvent>(ev =>
                {
                    targetRoot.Q<VisualElement>("SaveGameItemPreview").style.opacity = 0;
                });

                // Mouse
                btn.Q<VisualElement>("SavedGameEntry").RegisterCallback<MouseEnterEvent>(ev =>
                {
                    ShowLoadInfo(targetRoot, saveFileEntry);
                });
                btn.Q<VisualElement>("SavedGameEntry").RegisterCallback<MouseLeaveEvent>(ev =>
                {
                    targetRoot.Q<VisualElement>("SaveGameItemPreview").style.opacity = 0;
                });

                targetRoot.Q<ScrollView>().Add(btn);
            }

        }

        void ShowLoadInfo(VisualElement root, SaveFileEntry saveFileEntry)
        {

            root.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(saveFileEntry.screenshot);
            root.Q<Label>("Title").text = saveFileEntry.sceneName + " / Level " + saveFileEntry.level;
            root.Q<Label>("SubTitle").text = "Saved at: " + System.DateTime.Parse(saveFileEntry.creationDate).ToString();

            var totalGameTime = System.TimeSpan.FromSeconds(saveFileEntry.gameTime);
            root.Q<Label>("Description").text = "Total Play Time: " + totalGameTime.Hours + "h " + totalGameTime.Minutes + "m " + totalGameTime.Seconds + "s";
            root.Q<Label>("CurrentObjectiveText").text = "" + saveFileEntry.currentObjective;

            root.Q<VisualElement>("SaveGameItemPreview").style.opacity = 1;
        }

    }

}