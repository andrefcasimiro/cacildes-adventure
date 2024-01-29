using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "NPCs / Characters / New Character")]
    public class Character : ScriptableObject
    {
        public new string name;
        public string title;
        public Sprite avatar;
    }
}
