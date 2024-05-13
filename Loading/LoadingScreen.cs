using System.Collections.Generic;
using UnityEngine;

namespace AF.Loading
{
    [CreateAssetMenu(menuName = "System / New Loading Screen")]

    public class LoadingScreen : ScriptableObject
    {
        public Sprite image;
        [TextAreaAttribute(minLines: 5, maxLines: 10)] public string text;

        [Header("Settings")]
        public string[] mapNames;


    }
}
