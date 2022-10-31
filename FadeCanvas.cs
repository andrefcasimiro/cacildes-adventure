using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AF
{
    public class FadeCanvas : MonoBehaviour
    {
        public Image image;

        private void Start()
        {
            this.gameObject.SetActive(false);
        }

        public void FadeIn(float fadeInTime)
        {
            this.gameObject.SetActive(true);

            StartCoroutine(_FadeIn(fadeInTime));
        }
        public void FadeOut(float fadeOutTime)
        {

            this.gameObject.SetActive(true);

            StartCoroutine(_FadeOut(fadeOutTime));
        }

        public IEnumerator _FadeIn(float fadeTime)
        {
            image.color = new Color(0, 0, 0, 1);

            for (float i = 1; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                image.color = new Color(0, 0, 0, i);
                yield return new WaitForEndOfFrame();
            }

            this.gameObject.SetActive(false);
        }

        public IEnumerator _FadeOut(float fadeTime)
        {
            image.color = new Color(0, 0, 0, 0);

            this.gameObject.SetActive(true);

            // loop over 1 second backwards
            for (float i = 0; i <= 1; i += Time.deltaTime)
            {
                // set color with i as alpha
                image.color = new Color(0, 0, 0, i);
                yield return new WaitForEndOfFrame();
            }
        }

    }

}
