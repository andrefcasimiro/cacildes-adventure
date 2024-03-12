using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class FadeManager : MonoBehaviour
    {
        public CanvasGroup canvasGroup;

        [Header("Settings")]
        public float sceneFadeInSpeed = 1f;

        private void Awake()
        {
            canvasGroup.alpha = 1;
        }

        void Start()
        {
            Invoke(nameof(FadeOutOnStart), 0f);
        }

        void FadeOutOnStart()
        {
            FadeOut(sceneFadeInSpeed);
        }


        public void FadeIn(float fadeDuration)
        {
            canvasGroup.DOFade(1f, fadeDuration);
        }

        public void FadeOut(float fadeDuration)
        {
            canvasGroup.DOFade(0f, fadeDuration);
        }

        public void FadeIn(float fadeDuration, UnityAction callback)
        {
            FadeIn(fadeDuration);

            HandleCallback(fadeDuration, callback);
        }

        public void FadeOut(float fadeDuration, UnityAction callback)
        {
            FadeOut(fadeDuration);
            HandleCallback(fadeDuration, callback);
        }

        void HandleCallback(float fadeDuration, UnityAction callback)
        {
            StartCoroutine(ExecuteCallback_Coroutine());

            IEnumerator ExecuteCallback_Coroutine()
            {
                yield return new WaitForSeconds(fadeDuration);
                callback?.Invoke();
            }
        }
    }

}
