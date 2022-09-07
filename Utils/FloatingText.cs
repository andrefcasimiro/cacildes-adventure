using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AF
{
    public class FloatingText : MonoBehaviour
    {
        TextMeshPro text => GetComponent<TextMeshPro>();
        public float timeBeforeDisappearing = .25f;
        public float duration = .8f;

        private void Start()
        {
            this.gameObject.SetActive(false);
        }

        public void Show(float damage)
        {
            this.gameObject.SetActive(true);
            text.text = "- " + damage;
            transform.localScale = new Vector3(1, 1, 1);
            StartCoroutine(Hide());
        }

        IEnumerator Hide()
        {
            yield return new WaitForSeconds(timeBeforeDisappearing);
         
            for (float i = duration; i >= 0; i -= Time.deltaTime)
            {
                transform.localScale = new Vector3(i, i, i);
                yield return null;
            }

        }
    }
}
