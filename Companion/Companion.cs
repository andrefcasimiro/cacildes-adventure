using System.Collections;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Companions / New Companion")]
    public class Companion : ScriptableObject
    {
        public string companionId;

        public Character character;
    }
}
