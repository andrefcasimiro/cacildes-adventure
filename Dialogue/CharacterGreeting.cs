using UnityEngine;

namespace AF.Dialogue
{
    public class CharacterGreeting : MonoBehaviour
    {

        [TextAreaAttribute(minLines: 5, maxLines: 10)]
        public string greeting = "";

        public float duration = 5f;
    }
}
