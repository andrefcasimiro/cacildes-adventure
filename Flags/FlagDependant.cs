using AF.Events;
using TigerForge;
using UnityEngine;

namespace AF.Flags
{
    public class FlagDependant : MonoBehaviour
    {
        [Header("Databases")]
        public FlagsDatabase flagsDatabase;

        [Header("ID")]
        public MonoBehaviourID monoBehaviourID;

        [Header("Conditions")]
        public bool requireToBeRegistered = true;

        [Header("Settings")]
        [Tooltip("If false, will only evaluate the condition on the start of the scene")] public bool listenForFlagsChanged = true;

        private void Awake()
        {
            Utils.UpdateTransformChildren(transform, false);
        }

        private void Start()
        {
            Evaluate();

            if (listenForFlagsChanged)
            {
                EventManager.StartListening(EventMessages.ON_FLAGS_CHANGED, Evaluate);
            }
        }

        public void Evaluate()
        {
            bool isActive = false;

            if (requireToBeRegistered && flagsDatabase.ContainsFlag(monoBehaviourID.ID))
            {
                isActive = true;
            }
            else if (requireToBeRegistered == false && !flagsDatabase.ContainsFlag(monoBehaviourID.ID))
            {
                isActive = true;
            }

            Utils.UpdateTransformChildren(transform, isActive);
        }
    }
}
