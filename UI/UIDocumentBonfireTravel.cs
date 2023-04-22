using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    [System.Serializable]
    public class BonfireLocation
    {
        public LocalizedText bonfireName;
        public Sprite image;

        public TeleportManager.SceneName sceneName;
        public string spawnGameObjectNameRef;
    }

    public class UIDocumentBonfireTravel : MonoBehaviour
    {
        public List<BonfireLocation> bonfireLocations = new();


        public VisualTreeAsset travelOptionAsset;
        public UIDocumentBonfireMenu uIDocumentBonfireMenu;

        VisualElement root;

        void TranslateRoot()
        {
        }


        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void Start()
        {
        }

        private void OnEnable()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            TranslateRoot();

            root.Q<Button>("CloseBtn").RegisterCallback<ClickEvent>(ev =>
            {
                uIDocumentBonfireMenu.gameObject.SetActive(true);
                this.gameObject.SetActive(false);
            });


            root.Q<ScrollView>().Clear();
            root.Q<IMGUIContainer>().style.opacity = 0;

            // Add callbacks
            foreach (var location in bonfireLocations)
            {
                if (Player.instance.unlockAllBonfires || Player.instance.unlockedBonfires.Contains(location.bonfireName.GetEnglishText()))
                {
                    var clonedBonfireOption = travelOptionAsset.CloneTree();
                    clonedBonfireOption.Q<Button>().text = location.bonfireName.GetText();
                    clonedBonfireOption.Q<Button>().RegisterCallback<MouseOverEvent>(ev =>
                    {
                        root.Q<IMGUIContainer>().style.backgroundImage = new StyleBackground(location.image);
                        root.Q<IMGUIContainer>().style.opacity = 1;
                    });
                    clonedBonfireOption.Q<Button>().RegisterCallback<MouseOutEvent>(ev =>
                    {
                        root.Q<IMGUIContainer>().style.opacity = 0;
                    });

                    clonedBonfireOption.Q<Button>().RegisterCallback<ClickEvent>(ev =>
                    {
                        TeleportManager.instance.Teleport(location.sceneName, location.spawnGameObjectNameRef);
                    });

                    root.Q<ScrollView>().Add(clonedBonfireOption);
                }

            }

            FindObjectOfType<GamepadCursor>(true).gameObject.SetActive(true);
        }
    }

}
