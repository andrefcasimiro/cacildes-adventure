using System.Collections;
using UnityEngine;

namespace AF
{
    public class SwitchListener : MonoBehaviour
    {
        public string switchUuid;

        [HideInInspector] public Switch _switch;

        public virtual void EvaluateSwitch() { }

    }
}
