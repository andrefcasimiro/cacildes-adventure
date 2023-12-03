using UnityEngine;
using UnityEngine.UI;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Item / New Item")]
    public class Item : ScriptableObject
    {


        [Header("General")]
        public Sprite sprite;
        [TextAreaAttribute(minLines: 5, maxLines: 10)] public string itemDescription;
        public float value = 0;
        public bool lostUponUse = true;



        public new LocalizedText name;

        [Header("Descriptions")]

        public LocalizedText description;

        public LocalizedText shortDescription;
    }
}
