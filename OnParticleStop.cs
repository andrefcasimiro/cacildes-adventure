using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class OnParticleStop : MonoBehaviour
    {
        public UnityEvent onParticleStopped;

        private void OnParticleSystemStopped()
        {
            onParticleStopped.Invoke();
        }
    }
}
