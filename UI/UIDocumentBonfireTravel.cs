using System.Collections.Generic;
using AF.Bonfires;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    [System.Serializable]
    public class BonfireLocation
    {
        public string bonfireId;
        public Sprite image;

        public string sceneName;
        public string spawnGameObjectNameRef;
    }

    public class UIDocumentBonfireTravel : MonoBehaviour
    {
        public List<BonfireLocation> bonfireLocations = new();

        [Header("Components")]
        public Soundbank soundbank;
        public CursorManager cursorManager;
        public PlayerManager playerManager;
        public TeleportManager teleportManager;

        [Header("UI Documents")]
        public UIDocument uIDocument;
        public VisualTreeAsset travelOptionAsset;
        public UIDocumentBonfireMenu uIDocumentBonfireMenu;


        [Header("Databases")]
        public BonfiresDatabase bonfiresDatabase;

        // Internal
        VisualElement root;

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


        void Close()
        {
            uIDocumentBonfireMenu.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            root = GetComponent<UIDocument>().rootVisualElement;

            root.Q<ScrollView>().Clear();
            root.Q<IMGUIContainer>().style.opacity = 0;

            // The exit button
            var exitOption = travelOptionAsset.CloneTree();
            exitOption.Q<Button>().text = "Return";

            UIUtils.SetupButton(exitOption.Q<Button>(), () =>
            {
                Close();
            },
            () =>
            {
                {
                    root.Q<IMGUIContainer>().style.opacity = 0;
                }
            },
            () =>
            {
                root.Q<IMGUIContainer>().style.opacity = 0;
            },
            true,
            soundbank);

            root.Q<ScrollView>().Add(exitOption);

            // Add callbacks
            foreach (var location in bonfireLocations)
            {
                if (bonfiresDatabase.unlockedBonfires.Contains(location.bonfireId))
                {
                    var clonedBonfireOption = travelOptionAsset.CloneTree();
                    clonedBonfireOption.Q<Button>().text = location.bonfireId;

                    UIUtils.SetupButton(clonedBonfireOption.Q<Button>(), () =>
                    {
                        teleportManager.Teleport(location.sceneName, location.spawnGameObjectNameRef);
                    },
                    () =>
                    {
                        {
                            root.Q<IMGUIContainer>().style.backgroundImage = new StyleBackground(location.image);
                            root.Q<IMGUIContainer>().style.opacity = 1;
                        }
                    },
                    () =>
                    {
                        root.Q<IMGUIContainer>().style.opacity = 0;
                    },
                    true,
                    soundbank);


                    root.Q<ScrollView>().Add(clonedBonfireOption);
                }

            }

            cursorManager.ShowCursor();
        }
    }

}
