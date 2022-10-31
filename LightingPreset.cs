using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Lighting Preset", menuName = "Day and Night / New Lighting Preset")]
    public class LightingPreset : ScriptableObject
    {
        public Gradient AmbientColor;
        public Gradient DirectionalColor;
        public Gradient FogColor;
    }

}