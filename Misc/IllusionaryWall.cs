using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class IllusionaryWall : MonoBehaviour
    {
        public bool hasBeenHit;
         Material material;
        public Material fadeMaterial;
        public float alpha;
        public float fadeTimer = 2.5f;
        bool hasDeactivatedColliders = false;

        public UnityEvent onHit;

        private void Start()
        {
            this.material = Instantiate(fadeMaterial);
        }

        private void Update()
        {
            if (hasBeenHit)
            {
                FadeIllusionaryWall();
            }
        }

        public void FadeIllusionaryWall()
        {
            GetComponent<MeshRenderer>().material = this.material;

            alpha = material.color.a;
            alpha = alpha - Time.deltaTime / fadeTimer;
            Color c = new Color(1, 1, 1, alpha);
            material.color = c;

            if (onHit != null)
            {
                onHit.Invoke();
            }

            if (hasDeactivatedColliders == false)
            {
                foreach (var col in GetComponents<Collider>())
                {
                    col.enabled = false;
                }

                hasDeactivatedColliders = true;

                Soundbank.instance.PlayIllusionaryWall();
            }

            if (alpha <= 0)
            {
                this.gameObject.SetActive(false);
            }
        }

    }

}
