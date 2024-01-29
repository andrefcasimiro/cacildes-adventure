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
            Reset();
        }

        public void Reset()
        {
            transform.localScale = new Vector3(0, 0, 0);
        }

        public void Show(float damage)
        {
            text.text = "- " + damage;
            transform.localScale = new Vector3(100, 100, 100);
            StartCoroutine(Hide());
        }

        public void ShowText(string textToDisplay)
        {
            StopAllCoroutines();
            Reset();

            text.text = textToDisplay;
            transform.localScale = new Vector3(100, 100, 100);
            StartCoroutine(Hide());
        }

        IEnumerator Hide()
        {
            yield return new WaitForSeconds(timeBeforeDisappearing);
         
            for (float i = duration; i >= 0; i -= Time.deltaTime)
            {
                transform.localScale = new Vector3(i * 100, i * 100, i * 100);
                yield return null;
            }

            // Fix bug where text is still showing in small
            transform.localScale = new Vector3(0, 0, 0);
        }
    }
}
