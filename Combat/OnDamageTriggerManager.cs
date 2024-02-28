using System.Linq;
using UnityEngine;

namespace AF
{
    public class OnDamageTriggerManager : OnDamageCollisionAbstractManager
    {
        public string[] tagsToDetect;

        public bool onTriggerEnter = true;
        public bool onTriggerStay = false;

        void OnTriggerEnter(Collider other)
        {
            if (!tagsToDetect.Contains(other.gameObject.tag))
            {
                return;
            }

            if (onTriggerEnter)
            {
                OnCollision(other.gameObject);
            }
        }

        void OnTriggerStay(Collider other)
        {
            if (!tagsToDetect.Contains(other.gameObject.tag))
            {
                return;
            }

            if (onTriggerStay)
            {
                OnCollision(other.gameObject);
            }
        }

    }
}
