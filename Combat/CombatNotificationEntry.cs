using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace AF
{

    public class CombatNotificationEntry : MonoBehaviour
    {

        public TextMeshPro textMeshPro => GetComponent<TextMeshPro>();

        public float maxDuration = 5f;
        public float currentDuration = 0f;

        private IObjectPool<CombatNotificationEntry> pool;

        public void SetPool(IObjectPool<CombatNotificationEntry> pool)
        {
            this.pool = pool;
        }

        private void OnEnable()
        {
            this.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            StartCoroutine(Resize());
        }

        IEnumerator Resize()
        {
            while (this.transform.localScale.x > 1f)
            {
                this.transform.localScale = new Vector3(this.transform.localScale.x - 0.01f, this.transform.localScale.y - 0.01f, this.transform.localScale.z - 0.01f);

                yield return null;
            }
        }

        private void Update()
        {
            currentDuration += Time.deltaTime;

            if (currentDuration >= 0.5f)
            {
                var newColor = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, maxDuration - currentDuration - 0.5f);
                textMeshPro.color = newColor;
            }

            if (currentDuration >= maxDuration)
            {
                pool.Release(this);
                return;
            }

            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }

}
