using UnityEngine;
using System.Collections;

namespace AF
{
    [System.Serializable]
    public class SceneMusic
    {
        public float startDelayTime = 0f;

        public AudioClip audioClip;
    }

    public class SceneSettings: MonoBehaviour
    {
        public SceneMusic[] sceneMusics;
        public AudioClip sceneAmbience;

        public void Start()
        {
            if (sceneAmbience != null)
            {
                BGMManager.instance.ambienceAudioSource.PlayOneShot(sceneAmbience);
            }

            PlaySceneMusic();
        }

        public void PlaySceneMusic()
        {
            StopAllCoroutines();

            StartCoroutine(DispatchSceneMusic());

            if (sceneAmbience == null)
            {
                BGMManager.instance.StopAmbience();
            }
            else
            {
                BGMManager.instance.PlayMusic(sceneAmbience);
            }
        }

        private IEnumerator DispatchSceneMusic()
        {
            foreach (var sceneMusic in sceneMusics)
            {
                yield return new WaitForSeconds(sceneMusic.startDelayTime);

                BGMManager.instance.PlayMusic(sceneMusic.audioClip);

                yield return new WaitForSeconds(sceneMusic.audioClip.length);

                BGMManager.instance.StopMusic();
            }

            PlaySceneMusic();
        }
    }

}
