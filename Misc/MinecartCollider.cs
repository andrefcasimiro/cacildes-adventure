using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class MinecartCollider : MonoBehaviour
    {
        public SwitchEntry switchToActivateUponCollision;
        public DestroyableParticle mineCartColliderExplosion;
        public AudioSource miningCartSoundsource;

        public string analyticsMessage;

        public void ActivateCart()
        {
            miningCartSoundsource.gameObject.SetActive(true);
            GetComponent<Animator>().Play("MinecartSlidingDown");
        }

        public void Explode()
        {
            Instantiate(mineCartColliderExplosion, transform.position, Quaternion.identity);
            SwitchManager.instance.UpdateSwitch(switchToActivateUponCollision, true, null);

            if (string.IsNullOrEmpty(analyticsMessage) == false)
            {
                FindObjectOfType<Analytics>(true).TrackAnalyticsEvent(analyticsMessage);
            }
        }
    }
}
