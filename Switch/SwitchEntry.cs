using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Data / New Switch")]
    public class SwitchEntry : ScriptableObject
    {
        [Header("Update Objective")]
        public bool updateObjective = false;
        public string newObjective = "";

        [TextArea]
        public string description;
    }

}
