using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_ManageRenderer : EventBase
    {
        public MeshRenderer meshRenderer;
        public bool isActive;


        public override IEnumerator Dispatch()
        {
            yield return null;

            meshRenderer.enabled = isActive;
        }
    }

}
