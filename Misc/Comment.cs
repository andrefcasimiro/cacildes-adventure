using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class Comment : MonoBehaviour
    {
        [TextArea(1, 200)]
        public string text;
    }
}

