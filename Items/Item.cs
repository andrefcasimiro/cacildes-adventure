using UnityEngine;
using UnityEngine.UI;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Item / New Item")]
    public class Item : ScriptableObject
    {
        [TextArea] [SerializeField] private string comment = "The english item name and file name must match";

        public Sprite sprite;

        public new LocalizedText name;

        public LocalizedText description;

        public float value = 0;

        public float weight = 0;

    }

}
