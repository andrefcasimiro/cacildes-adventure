using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Data / New Variable")]
    public class VariableEntry : ScriptableObject
    {
        public int value = -1;

        [TextArea]
        public string description;
    }

}
