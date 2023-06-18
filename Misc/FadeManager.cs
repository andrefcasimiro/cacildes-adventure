using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AF
{
    public class FadeManager : MonoBehaviour
    {
        public Canvas canvas;
        public Image image;

        public float fadeSpeed;
        
        public void Fade()
        {
            StartCoroutine(BeginFade());
        }

        IEnumerator BeginFade()
        {
            canvas.sortingOrder = 999;
            while (image.color.a < 1)
            {
                var newColor = new Color(image.color.r, image.color.g, image.color.b, image.color.a + Time.deltaTime * fadeSpeed);
                image.color = newColor;
                yield return null;
            }

            StartCoroutine(EndFade());
        }

        IEnumerator EndFade()
        {
            yield return new WaitForSeconds(1f);

            while (image.color.a > 0)
            {
                var newColor = new Color(image.color.r, image.color.g, image.color.b, image.color.a - Time.deltaTime * fadeSpeed);
                image.color = newColor;
                yield return null;
            }

            canvas.sortingOrder = 0;
        }

    }

}
