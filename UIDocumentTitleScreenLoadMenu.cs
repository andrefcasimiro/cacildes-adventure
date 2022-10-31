using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{

    public class UIDocumentTitleScreenLoadMenu : MonoBehaviour
    {
        VisualElement root;
        public VisualTreeAsset loadButtonEntryPrefab;

        private void Awake()
        {
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            this.root = GetComponent<UIDocument>().rootVisualElement;
            DrawUI();
        }

        void DrawUI()
        {

            List<SaveFileEntry> saveFiles = SaveSystem.instance.GetSaveFiles();

            root.Q<ScrollView>().Clear();
            root.Q<VisualElement>("ItemPreview").style.opacity = 0;

            root.Q<Button>("CloseBtn").RegisterCallback<ClickEvent>(ev =>
            {
                FindObjectOfType<UIDocumentTitleScreen>(true).gameObject.SetActive(true);
                this.gameObject.SetActive(false);
            });
            root.Q<Button>("ButtonOpenSaveFolder").RegisterCallback<ClickEvent>(ev =>
            {
                string path = System.IO.Path.Combine(Application.persistentDataPath);
                var itemPath = path.Replace(@"/", @"\");   // explorer doesn't like front slashes
                System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
            });

            foreach (var saveFileEntry in saveFiles)
            {
                var btn = loadButtonEntryPrefab.CloneTree();

                btn.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(saveFileEntry.screenshot);

                btn.Q<Label>("Name").text = (saveFileEntry.isQuickSave ? "Quick Save / " : "") + saveFileEntry.sceneName + " / Lv " + saveFileEntry.level;
                btn.Q<Label>("Value").text = System.DateTime.Parse(saveFileEntry.creationDate).ToString();
                btn.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(saveFileEntry.screenshot);

                // btn.Q<Button>("DeleteButton").style.display = saveFileEntry.fileFullPath.ToLower().Contains("quicksave") ? DisplayStyle.None : DisplayStyle.Flex;

                btn.Q<Button>("DeleteButton").RegisterCallback<ClickEvent>(ev =>
                {
                    SaveSystem.instance.LoadGameData(saveFileEntry.fileFullPath);

                    this.gameObject.SetActive(false);
                });
                btn.Q<Button>("DeleteButton").text = "Load";

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

        private void Update()
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }

    }

}