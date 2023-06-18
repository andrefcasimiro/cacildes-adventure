using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class TrickerCloud : MonoBehaviour
    {
        public float timeActive = 5;


        public float speed = 0.3f;

        float alpha;

        Renderer meshRenderer => GetComponent<Renderer>();
        Material copiedMaterial;

        float stoppedTime;

        private void Start()
        {
            copiedMaterial = Instantiate(meshRenderer.material);
            meshRenderer.material = copiedMaterial;
        }

        public void Update()
        {
            if (stoppedTime < timeActive)
            {
                stoppedTime += Time.deltaTime;
                return;
            }

            alpha = meshRenderer.material.color.a;
         
            if (alpha <= 0)
            {

                while (alpha < 1)
                {
                    alpha += Time.deltaTime / speed;
                    copiedMaterial.color = new Color(1, 1, 1, alpha);
                    GetComponent<MeshRenderer>().material = copiedMaterial;
                }

                GetComponent<MeshCollider>().enabled = true;
                stoppedTime = 0f;

            }
            else if (alpha >= 1)
            {

                while (alpha > 0)
                {
                    alpha -= Time.deltaTime / speed;
                    Color c = new Color(1, 1, 1, alpha);
                    copiedMaterial.color = c;
                    GetComponent<MeshRenderer>().material = copiedMaterial;
                }


                GetComponent<MeshCollider>().enabled = false;
                stoppedTime = 0f;
            }
        }

    }

}
