using UnityEngine;
using UnityEngine.UI;

namespace AF
{
    [CreateAssetMenu(menuName = "Item / New Item")]
    public class Item : ScriptableObject
    {
        public Sprite sprite;

        public string name;

        [TextArea]
        public string description;

    }

}
