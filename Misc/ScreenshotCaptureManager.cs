using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace AF
{
    public class ScreenshotCaptureManager : MonoBehaviour
    {
        public Texture2D fallbackSaveScreenshot;

        public Texture2D CaptureScreenshot()
        {
            RenderTexture renderTexture = null;
            try
            {
                renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
                Camera.main.targetTexture = renderTexture;
                Camera.main.Render();

                RenderTexture.active = renderTexture;
                Texture2D screenshotTexture = new Texture2D(Screen.width, Screen.height);
                screenshotTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                screenshotTexture.Apply();

                return screenshotTexture;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to capture screenshot: {ex.Message}");
                return fallbackSaveScreenshot;
            }
            finally
            {
                // Cleanup
                RenderTexture.active = null;
                Camera.main.targetTexture = null;
                if (renderTexture != null)
                    Destroy(renderTexture);
            }
        }

    }
}
