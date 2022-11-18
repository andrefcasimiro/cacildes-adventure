using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{

    public class UIDocumentLoadMenu : MonoBehaviour
    {
        MenuManager menuManager;
        VisualElement root;

        public VisualTreeAsset loadButtonEntryPrefab;

        private void Awake()
        {
            menuManager = FindObjectOfType<MenuManager>(true);
        }

        private void OnEnable()
        {
            this.root = GetComponent<UIDocument>().rootVisualElement;
            menuManager.SetupNavMenu(this.root);
            menuManager.SetActiveMenu(this.root, "ButtonLoad");

            DrawUI();
        }

        void DrawUI()
        {

            List<SaveFileEntry> saveFiles = SaveSystem.instance.GetSaveFiles();

            root.Q<ScrollView>().Clear();
            root.Q<VisualElement>("ItemPreview").style.opacity = 0;

            foreach (var saveFileEntry in saveFiles)
            {
                var btn = loadButtonEntryPrefab.CloneTree();

                btn.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(saveFileEntry.screenshot);

                btn.Q<Label>("Name").text = (saveFileEntry.isQuickSave ? "Quick Save / " : "") + saveFileEntry.sceneName + " / Lv " + saveFileEntry.level;
                btn.Q<Label>("Value").text = System.DateTime.Parse(saveFileEntry.creationDate).ToString();
                btn.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(saveFileEntry.screenshot);

                var loadBtn = btn.Q<Button>("DeleteButton");

                menuManager.SetupButton(loadBtn, () =>
                {
                    SaveSystem.instance.LoadGameData(saveFileEntry.fileFullPath);
                    menuManager.CloseMenu();
                });

                btn.Q<Button>("DeleteButton").text = "Load";

                // Gamepad
                btn.RegisterCallback<FocusInEvent>(ev =>
                {
                    ShowLoadInfo(btn, saveFileEntry);
                });
                btn.RegisterCallback<FocusOutEvent>(ev =>
                {
                    root.Q<VisualElement>("ItemPreview").style.opacity = 0;
                });

                // Mouse
                btn.RegisterCallback<PointerEnterEvent>(ev =>
                {
                    ShowLoadInfo(btn, saveFileEntry);
                });
                btn.RegisterCallback<PointerOutEvent>(ev =>
                {
                    root.Q<VisualElement>("ItemPreview").style.opacity = 0;
                });

                root.Q<ScrollView>().Add(btn);
            }

        }

        void ShowLoadInfo(VisualElement btn, SaveFileEntry saveFileEntry)
        {

            root.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(saveFileEntry.screenshot);
            root.Q<Label>("Title").text = saveFileEntry.sceneName + ", " + saveFileEntry.level;
            root.Q<Label>("SubTitle").text = System.DateTime.Parse(saveFileEntry.creationDate).ToString();

            var totalGameTime = System.TimeSpan.FromSeconds(saveFileEntry.gameTime);
            root.Q<Label>("Description").text = "Total Game Time: " + totalGameTime.Hours + "h:" + totalGameTime.Minutes + "m:" + totalGameTime.Seconds + "s";

            root.Q<VisualElement>("ItemPreview").style.opacity = 1;
            root.Q<ScrollView>().ScrollTo(btn);
        }

    }

}