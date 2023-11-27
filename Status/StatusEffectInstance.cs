
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    [System.Serializable]
    public class StatusEffectInstance : MonoBehaviour
    {
        public StatusEffect statusEffect;

        [Header("Events")]
        public UnityEvent onApplied_Start;
        public UnityEvent onApplied_Update;
        public UnityEvent onApplied_End;

    }
}
