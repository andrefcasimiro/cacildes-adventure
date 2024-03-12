using System.Collections;
using AF.Events;
using TigerForge;
using UnityEngine;

namespace AF
{
    public class UltraGraphics : MonoBehaviour
    {
        private void Awake()
        {
            EventManager.StartListening(EventMessages.ON_GRAPHICS_QUALITY_CHANGED, Evaluate);
        }

        void Evaluate()
        {
            Utils.UpdateTransformChildren(transform, QualitySettings.GetQualityLevel() >= 3);
        }
    }
}
