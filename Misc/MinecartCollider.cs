using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class MinecartCollider : MonoBehaviour
    {
        public SwitchEntry switchEntry;
        public DestroyableParticle mineCartColliderExplosion;
        public AudioSource miningCartSoundsource;
 
        public UnityEvent onActivateCartEvent;

        public void ActivateCart()
        {
            onActivateCartEvent.Invoke();
            miningCartSoundsource.gameObject.SetActive(true);
            GetComponent<Animator>().Play("MinecartSlidingDown");
        }

        public void Explode()
        {
            Instantiate(mineCartColliderExplosion, transform.position, Quaternion.identity);
            SwitchManager.instance.UpdateSwitch(switchEntry, true);
        }
    }
}
