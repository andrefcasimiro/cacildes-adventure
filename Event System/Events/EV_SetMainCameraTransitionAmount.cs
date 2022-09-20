using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_SetMainCameraTransitionAmount : EventBase
    {

        public float amount;

        public override IEnumerator Dispatch()
        {
            Camera.main.GetComponent<CameraTransitionManager>().SetTransition(amount);
            yield return null;
        }
    }

}
