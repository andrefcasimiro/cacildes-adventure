using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class EnemyMaterialManagerHelper : MonoBehaviour
    {
        [HideInInspector]
        public Material originalMaterial;
        public Material materialWhenInvulnerable;

        public SkinnedMeshRenderer targetMesh;

        public float maxTime = 10f;
        float cooldown = Mathf.Infinity;
        bool isActive = false;

        public UnityEvent onActivate;
        public UnityEvent onDeactivate;

        private void Start()
        {
            this.originalMaterial = targetMesh.material;
        }

        public void Activate()
        {
            cooldown = 0f;
            isActive = true;
            targetMesh.material = materialWhenInvulnerable;
            onActivate.Invoke();
        }

        public void Deactivate()
        {

            isActive = false;
            targetMesh.material = originalMaterial;
            onDeactivate.Invoke();
        }

        public void Update()
        {

            if (cooldown < maxTime)
            {
                cooldown += Time.deltaTime;
            }

            if (isActive && cooldown >= maxTime)
            {
                Deactivate();
            }
        }

    }

}
