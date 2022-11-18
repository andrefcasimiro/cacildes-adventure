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
        MenuManager menuManager;

        private void Awake()
        {
            menuManager = FindObjectOfType<MenuManager>(true);

            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            this.root = GetComponent<UIDocument>().rootVisualElement;
            
            root.RegisterCallback<NavigationCancelEvent>(ev =>
            {
                Close();
            });

            DrawUI();
        }

        void DrawUI()
        {

            List<SaveFileEntry> saveFiles = SaveSystem.instance.GetSaveFiles();

            root.Q<ScrollView>().Clear();
            root.Q<VisualElement>("ItemPreview").style.opacity = 0;

            menuManager.SetupButton(root.Q<Button>("CloseBtn"), () =>
            {
                Close();
            });


            menuManager.SetupButton(root.Q<Button>("ButtonOpenSaveFolder"), () =>
            {
                string path = System.IO.Path.Combine(Application.persistentDataPath);
                var itemPath = path.Replace(@"/", @"\");   // explorer doesn't like front slashes
                System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
            });

            Button focusedButton = null;

            foreach (var saveFileEntry in saveFiles)
            {
                var btn = loadButtonEntryPrefab.CloneTree();

                btn.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(saveFileEntry.screenshot);

                btn.Q<Label>("Name").text = (saveFileEntry.isQuickSave ? "Quick Save / " : "") + saveFileEntry.sceneName + " / Lv " + saveFileEntry.level;
                btn.Q<Label>("Value").text = System.DateTime.Parse(saveFileEntry.creationDate).ToString();
                btn.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(saveFileEntry.screenshot);
                
                var btnLoad = btn.Q<Button>("DeleteButton");
                if (focusedButton == null)
                {
                    focusedButton = btnLoad;
                }
                
                menuManager.SetupButton(btnLoad, () =>
                {
                    SaveSystem.instance.LoadGameData(saveFileEntry.fileFullPath);

                    this.gameObject.SetActive(false);
                });


                btnLoad.text = "Load";

                // GamePad
                btn.RegisterCallback<FocusInEvent>(ev =>
                {
                    ShowLoad(btn, saveFileEntry);
                });

                btn.RegisterCallback<FocusOutEvent>(ev =>
                {
                    root.Q<VisualElement>("ItemPreview").style.opacity = 0;
                });

                // Mouse
                btn.RegisterCallback<PointerEnterEvent>(ev =>
                {
                    ShowLoad(btn, saveFileEntry);
                });

                btn.RegisterCallback<PointerOutEvent>(ev =>
                {
                    root.Q<VisualElement>("ItemPreview").style.opacity = 0;
                });

                root.Q<ScrollView>().Add(btn);
            }


        }

        void ShowLoad(VisualElement btn, SaveFileEntry saveFileEntry)
        {

            root.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(saveFileEntry.screenshot);
            root.Q<Label>("Title").text = saveFileEntry.sceneName + ", " + saveFileEntry.level;
            root.Q<Label>("SubTitle").text = System.DateTime.Parse(saveFileEntry.creationDate).ToString();

            var totalGameTime = System.TimeSpan.FromSeconds(saveFileEntry.gameTime);
            root.Q<Label>("Description").text = "Total Game Time: " + totalGameTime.Hours + "h:" + totalGameTime.Minutes + "m:" + totalGameTime.Seconds + "s";

            root.Q<VisualElement>("ItemPreview").style.opacity = 1;

            root.Q<ScrollView>().ScrollTo(btn);
        }

        void Close()
        {
            FindObjectOfType<UIDocumentTitleScreen>(true).gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }

        private void Update()
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }

    }

}