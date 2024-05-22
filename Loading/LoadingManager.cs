using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        // Static instance variable
        private static LoadingManager instance;

        // Singleton instance property
        public static LoadingManager Instance => instance;

        private void Awake()
        {
            // Ensure only one instance of LoadingManager exists
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            DontDestroyOnLoad(gameObject);

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
            SetupLoadingUI(sceneName);
            SceneManager.LoadScene(sceneName);
        }

        public void BeginLoading(int sceneIndex)
        {
            SetupLoadingUI("");
            SceneManager.LoadScene(sceneIndex);
        }

        void SetupLoadingUI(string sceneName)
        {
            loadingScreen.enabled = true;

            SetupRefs();

            //loadingFill.style.width = new Length(0, LengthUnit.Percent);

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

            UpdateLoading(0);
        }


        public void UpdateLoading(float currentProgress)
        {
            //loadingFill.style.width = new Length(currentProgress, LengthUnit.Percent);
        }

        public void EndLoading()
        {
            if (root == null)
            {
                return;
            }

            //loadingFill.style.width = new Length(100, LengthUnit.Percent);
            loadingScreen.enabled = false;
        }
    }
}
