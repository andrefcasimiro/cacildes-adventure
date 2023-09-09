using System.Collections;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Data / New Achievement")]

    public class Achievement : ScriptableObject
    {

        public LocalizedText achievementName;

        public LocalizedText description;

        public Texture2D icon;

    }
}
