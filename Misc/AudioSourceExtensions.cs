using System.Collections;

namespace UnityEngine
{
    public static class AudioSourceExtensions
    {
        public static void FadeOut(this AudioSource a, float duration)
        {
            a.GetComponent<MonoBehaviour>().StartCoroutine(FadeOutCore(a, duration));
        }

        private static IEnumerator FadeOutCore(AudioSource a, float duration)
        {
            float startVolume = a.volume;

            while (a.volume > 0)
            {
                a.volume -= startVolume * Time.deltaTime / duration;
                yield return new WaitForEndOfFrame();
            }

            a.Stop();
            a.volume = startVolume;
        }


        public static void FadeIn(this AudioSource a, float fadeInSpeed)
        {
            a.GetComponent<MonoBehaviour>().StartCoroutine(FadeInCore(a, fadeInSpeed));
        }

        private static IEnumerator FadeInCore(AudioSource a, float fadeInSpeed)
        {
            while (a.volume < 1)
            {
                a.volume += fadeInSpeed * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            a.Play();
        }
    }
}