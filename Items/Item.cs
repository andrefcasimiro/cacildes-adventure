using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Item / New Item")]
    public class Item : ScriptableObject
    {

        [Header("General")]
        public Sprite sprite;
        [TextAreaAttribute(minLines: 5, maxLines: 10)] public string itemDescription;
        public string shortDescription;

        [Header("Value")]
        public float value = 0;
        public bool lostUponUse = true;

        [Header("Debug")]
        [TextAreaAttribute(minLines: 5, maxLines: 10)] public string notes;

    }
}
