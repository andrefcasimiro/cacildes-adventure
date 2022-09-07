using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_FaceOriginalPosition : EventBase
    {
        public Transform npc;
        Quaternion originalRotation;

        private void Start()
        {
            originalRotation = npc.transform.rotation;
        }

        public override IEnumerator Dispatch()
        {
            npc.transform.rotation = originalRotation;

            yield return null;
        }
    }
}