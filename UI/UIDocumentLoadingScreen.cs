using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace AF
{

    public class UIDocumentLoadingScreen : MonoBehaviour
    {

        VisualElement root;

        public float fadeTime = 2f;

        public void UpdateLoadingBar(float loadingPercentage)
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            root.Q<Label>("LoadingText").text = LocalizedTerms.Loading()+"...";
            root.Q<VisualElement>("LoadingFill").style.width = new Length(loadingPercentage, LengthUnit.Percent);
        }

        public IEnumerator FadeAndDisable()
        {
            if (root == null)
            {
                root = GetComponent<UIDocument>().rootVisualElement;
            }

            root.Q<VisualElement>("LoadingBar").style.opacity = 0f;

            var el = root.Q<VisualElement>("Background");
            el.style.opacity = 1f;
            while (el.style.opacity.value > 0f)
            {
                el.style.opacity = el.style.opacity.value + (Time.deltaTime * -fadeTime);

                yield return null;
            }

            yield return null;

            this.gameObject.SetActive(false);
        }
    }

}