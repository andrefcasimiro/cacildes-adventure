using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class StalactiteCeilingManager : MonoBehaviour
    {
        public Cinemachine.CinemachineImpulseSource cinemachineImpulseSource;

        public void ShakeCamera()
        {

            // Rotate randomly
            var targetRot = Random.rotation;
            targetRot.x = 0;
            targetRot.z = 0;

            cinemachineImpulseSource.GenerateImpulse();

        }

        public void Launch()
        {
            Stalactite[] stalactites = transform.GetComponentsInChildren<Stalactite>();

            foreach (Stalactite t in stalactites)
            {
                t.Drop();
            }
        }

    }
}
