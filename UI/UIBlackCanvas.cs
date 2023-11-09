using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{

    public class UIBlackCanvas : MonoBehaviour, ISaveable
    {

        public float fadeTime = 2f;

        private void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            var el = root.Q<VisualElement>("Background");
            el.style.opacity = 1f;
        }

        public void StartFade()
        {
            StartCoroutine(FadeAndDisable());
        }

        private IEnumerator FadeAndDisable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            if (root != null)
            {
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

        public void OnGameLoaded(object gameData)
        {
            OnReset();
        }

        public void OnReset()
        {
            this.gameObject.SetActive(false);
        }
    }

}