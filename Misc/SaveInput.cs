using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF
{
    public class SaveInput : MonoBehaviour
    {
        public StarterAssets.StarterAssetsInputs inputs;

        MenuManager menuManager;

        private void Start()
        {
            menuManager = FindObjectOfType<MenuManager>(true);
        }

        private void Update()
        {
            if (inputs.quickSave)
            {
                inputs.quickSave = false;

                SaveSystem.instance.currentScreenshot = ScreenCapture.CaptureScreenshotAsTexture();

                SaveSystem.instance.SaveGameData(SceneManager.GetActiveScene().name);
            }

            if (inputs.quickLoad)
            {
                inputs.quickLoad = false;

                SaveSystem.instance.LoadLastSavedGame();
            }
        }
    }

}
