using AF.Events;
using TigerForge;
using UnityEngine;

namespace AF.Conditions
{
    public class BodyTypeDependant : MonoBehaviour
    {
        public bool TrueIfMale = false;
        public bool TrueIfFemale = false;

        public PlayerManager playerManager;

        private void Awake()
        {
            EventManager.StartListening(EventMessages.ON_BODY_TYPE_CHANGED, Evaluate);
        }

        private void OnEnable()
        {
            Evaluate();
        }

        public void Evaluate()
        {
            bool isActive = false;

            if (playerManager.playerAppearance.GetBodyType() == 0 && TrueIfMale)
            {
                isActive = true;
            }
            else if (playerManager.playerAppearance.GetBodyType() == 1 && TrueIfFemale)
            {
                isActive = true;
            }

            Utils.UpdateTransformChildren(transform, isActive);
        }
    }
}
