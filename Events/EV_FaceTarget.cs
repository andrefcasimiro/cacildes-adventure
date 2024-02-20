using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_FaceTarget : EventBase
    {
        public Transform character;

        [Header("Options")]
        public Transform lookTarget;
        public bool facePlayer = false;
        public bool faceTheOtherWayAround = false;

        public override IEnumerator Dispatch()
        {
            if (faceTheOtherWayAround)
            {
                character.transform.rotation = Quaternion.LookRotation(
                    character.transform.forward * -1f);
            }
            else
            {
                if (facePlayer)
                {
                    lookTarget = GameObject.FindWithTag("Player").transform;
                }

                var lookPos = lookTarget.transform.position - character.transform.position;
                lookPos.y = 0;
                character.transform.rotation = Quaternion.LookRotation(lookPos);
            }

            yield return null;
        }
    }
}