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

        float maxSavedGameCoooldownTimer = 1f;
        float savedGameCooldownTimer = Mathf.Infinity;
        
        Button btnSave;


        private void Awake()
        {
            menuManager = FindObjectOfType<MenuManager>(true);
        }

        private void OnEnable()
        {
            this.root = GetComponent<UIDocument>().rootVisualElement;
            menuManager.SetupNavMenu(root);

            this.btnSave = root.Q<Button>("SaveGameButton");
            menuManager.SetupButton(this.btnSave, () => {
                if (savedGameCooldownTimer > maxSavedGameCoooldownTimer)
                {
                    SaveSystem.instance.SaveGameData(SaveSystem.instance.SAVE_FILE_NAME + System.DateTime.Now.ToFileTimeUtc());
                    DrawUI();
                    savedGameCooldownTimer = 0f;
                }
            });
            
            DrawUI();
        }

        private void Update()
        {
            if (savedGameCooldownTimer < maxSavedGameCoooldownTimer)
            {
                savedGameCooldownTimer += Time.deltaTime;
            }
        }

        void DrawUI()
        {
            menuManager.SetActiveMenu(root, "ButtonSave");

            List<SaveFileEntry> saveFiles = SaveSystem.instance.GetSaveFiles();

            bool saveIsAllowed = saveFiles.Count <= SaveSystem.instance.MAX_SAVE_FILES_ALLOWED;

            root.Q<Button>("SaveGameButton").SetEnabled(saveIsAllowed);

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

                var deleteBtn = btn.Q<Button>("DeleteButton");
                menuManager.SetupButton(deleteBtn, () =>
                {
                    SaveSystem.instance.DeleteSaveFile(saveFileEntry.fileFullPath, saveFileEntry.fileFullPath.Replace("json", "jpg"));

                    DrawUI();
                });

                // Gamepad 

                btn.RegisterCallback<FocusInEvent>(ev =>
                {
                    ShowSavePreview(btn, saveFileEntry);
                });

                btn.RegisterCallback<FocusOutEvent>(ev =>
                {
                    root.Q<VisualElement>("ItemPreview").style.opacity = 0;
                });

                // Mouse 

                btn.RegisterCallback<PointerEnterEvent>(ev =>
                {
                    ShowSavePreview(btn, saveFileEntry);
                });

                btn.RegisterCallback<PointerOutEvent>(ev =>
                {
                    root.Q<VisualElement>("ItemPreview").style.opacity = 0;
                });

                root.Q<ScrollView>().Add(btn);
            }

        }

        void ShowSavePreview(VisualElement btn, SaveFileEntry saveFileEntry)
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