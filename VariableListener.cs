using System.Collections;
using UnityEngine;

namespace AF
{
    public class VariableListener : MonoBehaviour
    {
        public string variableUuid;

        protected Variable _variable;

        public virtual void EvaluateVariable() { }

    }
}