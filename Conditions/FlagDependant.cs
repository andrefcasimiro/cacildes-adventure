using AF.Events;
using AF.Flags;
using TigerForge;
using UnityEngine;

namespace AF.Conditions
{
    public class FlagDependant : MonoBehaviour
    {
        [Header("Databases")]
        public FlagsDatabase flagsDatabase;

        [Header("Monobehaviour ID Option")]
        public MonoBehaviourID monoBehaviourID;

        [Header("Flag Option")]
        public Flag flag;

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

            string id = "";
            if (monoBehaviourID != null)
            {
                id = monoBehaviourID.ID;
            }
            else if (flag != null)
            {
                id = flag.name;
            }

            if (requireToBeRegistered && flagsDatabase.ContainsFlag(id))
            {
                isActive = true;
            }
            else if (requireToBeRegistered == false && !flagsDatabase.ContainsFlag(id))
            {
                isActive = true;
            }

            Utils.UpdateTransformChildren(transform, isActive);
        }
    }
}
