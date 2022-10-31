using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

                menuManager.screenshotBeforeOpeningMenu = ScreenCapture.CaptureScreenshotAsTexture();

                SaveSystem.instance.SaveGameData(SaveSystem.instance.QUICK_SAVE_FILE_NAME);
            }

            if (inputs.quickLoad)
            {
                inputs.quickLoad = false;

                SaveSystem.instance.LoadLastSavedGame();
            }
        }
    }

}
