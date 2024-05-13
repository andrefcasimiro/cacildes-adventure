using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF.Loading
{
    public class LoadingManager : MonoBehaviour
    {
        public UIDocument loadingScreen;

        VisualElement loadingFill;
        VisualElement tipImage;
        Label tipText;

        public LoadingScreen[] loadingScreens;

        VisualElement root;

        private void Awake()
        {
            loadingScreen.enabled = false;

            SetupRefs();
        }

        private void OnEnable()
        {
            SetupRefs();
        }

        void SetupRefs()
        {
            root = loadingScreen.rootVisualElement;

            if (root != null)
            {
                loadingFill = root.Q<VisualElement>("LoadingFill");
                tipImage = root.Q<VisualElement>("TipImage");
                tipText = root.Q<Label>("TipLabel");
            }
        }


        public void BeginLoading(string sceneName)
        {
            loadingScreen.enabled = true;

            SetupRefs();

            loadingFill.style.width = new Length(0, LengthUnit.Percent);

            var candidates = loadingScreens.Where(x => x.mapNames.Contains(sceneName)).ToArray();

            LoadingScreen candidate;
            if (candidates.Length > 0 && Random.Range(0, 1f) > 0.6f)
            {
                candidate = candidates[Random.Range(0, candidates.Length)];
            }
            else
            {
                var candidatesWithNoMapName = loadingScreens.Where(x => x.mapNames.Length <= 0).ToArray();
                candidate = candidatesWithNoMapName[Random.Range(0, candidatesWithNoMapName.Length)];
            }

            tipImage.style.backgroundImage = new StyleBackground(candidate.image);
            tipText.text = candidate.text;
        }

        public void UpdateLoading(float currentProgress)
        {
            loadingFill.style.width = new Length(currentProgress, LengthUnit.Percent);
        }

    }
}
