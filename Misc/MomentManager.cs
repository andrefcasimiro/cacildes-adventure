using AF.Events;
using TigerForge;
using UnityEngine;

namespace AF
{
    public class MomentManager : MonoBehaviour
    {
        private bool hasMomentOnGoing = false;

        public bool HasMomentOnGoing
        {
            get => hasMomentOnGoing;
            set => hasMomentOnGoing = value;
        }

        private void Awake()
        {
            EventManager.StartListening(EventMessages.ON_MOMENT_START, () => { HasMomentOnGoing = true; });
            EventManager.StartListening(EventMessages.ON_MOMENT_END, () => { HasMomentOnGoing = false; });
        }
    }
}
