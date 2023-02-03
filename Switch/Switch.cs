using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class Switch : MonoBehaviour
    {
        public string ID;

        public string name;

        public bool value;

        [TextArea]
        public string description;

        [Header("Update Objective")]
        public bool updateObjective = false;
        public string newObjective = "";

    }

}