using UnityEngine;
using UnityEngine.UI;

namespace AF
{
    [CreateAssetMenu(menuName = "Item / New Item")]
    public class Item : ScriptableObject
    {
        public Sprite sprite;

        public string name;

        public string smallEffectDescription;

        [TextArea]
        public string description;

        public int value = 0;

        public float weight = 0;

    }

}
