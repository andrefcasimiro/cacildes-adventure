using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class EnemyBuffEventManager : MonoBehaviour
    {
        public float duration = 10;

        public UnityEvent onActivate;
        public UnityEvent onDeactivate;

        [Header("Grow Options")]
        public Vector3 growSize;
        public Vector3 shrinkSize;
        public float growShrinkSpeed = 2f;
        Vector3 originalScale;
        Vector3 targetScale;

        private void Start()
        {
            originalScale = transform.localScale;
        }

        public void Activate()
        {
            onActivate.Invoke();
            StartCoroutine(CallDeactivate());
        }

        IEnumerator CallDeactivate()
        {
            yield return new WaitForSeconds(duration);
            Deactivate();
        }

        public void Deactivate()
        {
            onDeactivate.Invoke();
        }

        public void GrowOrShrink()
        {
            float random = Random.Range(0, 1f);
            if (random >= 0.5)
            {
                Grow();
            }
            else
            {
                Shrink();
            }
        }

        public void Grow()
        {
            targetScale = growSize;
        }

        public void Shrink()
        {
            targetScale = shrinkSize;
        }

        public void ReturnToNormalSize()
        {
            targetScale = originalScale;
        }

        private void Update()
        {
            if (targetScale != Vector3.zero)
            {
                if (transform.localScale.magnitude != targetScale.magnitude)
                {
                    transform.localScale = Vector3.Lerp(transform.localScale, targetScale, growShrinkSpeed * Time.deltaTime);
                }
                else
                {
                    targetScale = Vector3.zero;
                }

            }
        }


    }

}
