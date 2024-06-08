using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class OnParticleCollisionManager : OnDamageCollisionAbstractManager
    {
        public UnityEvent onAnyCollisionEvent;

        private void OnParticleCollision(GameObject other)
        {
            OnCollision(other);

            onAnyCollisionEvent?.Invoke();
        }
    }
}
