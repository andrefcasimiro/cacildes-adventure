using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class MinecartCollider : MonoBehaviour
    {
        public string switchUuid;
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
            Instantiate(mineCartColliderExplosion, this.transform.position, Quaternion.identity);
            SwitchManager.instance.UpdateSwitch(switchUuid, true);

        }

    }

}
