using System.Collections.Generic;
using UnityEngine;

namespace AF.Footsteps
{

    public class FootstepListener : MonoBehaviour
    {

        [Header("Footstep Receivers")]
        public FootstepReceiver leftFootReceiver;
        public FootstepReceiver rightFootReceiver;

        public void PlayFootstep(bool isLeft)
        {
            if (isLeft && leftFootReceiver != null)
            {
                leftFootReceiver.Trigger();
            }
            else if (rightFootReceiver != null)
            {
                rightFootReceiver.Trigger();
            }
        }

    }
}
