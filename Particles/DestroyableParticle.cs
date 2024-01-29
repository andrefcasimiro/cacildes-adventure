using System.Collections;
using UnityEngine;

namespace AF
{
    public class DestroyableParticle : MonoBehaviour
    {
        public float destroyAfter = 5f;


        // Use this for initialization
        void Start()
        {
            StartCoroutine(DestroySelf());
        }

        IEnumerator DestroySelf()
        {
            yield return new WaitForSeconds(destroyAfter);

            Destroy(this.gameObject);
        }

        private void OnDisable()
        {
            Destroy(this.gameObject);
        }

    }

}
