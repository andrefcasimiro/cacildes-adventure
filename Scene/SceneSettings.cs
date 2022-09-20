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

        public float fadeBetweenScenes = 1f;

        public void Start()
        {

            if (sceneAmbience == null)
            {
                BGMManager.instance.StopAmbience();
            }
            else
            {
                BGMManager.instance.PlayAmbience(sceneAmbience);
            }

            PlaySceneMusic();
        }

        public void PlaySceneMusic()
        {
            if (sceneMusics.Length <= 0)
            {
                return;
            }

            if (BGMManager.instance.bgmAudioSource.clip != null && BGMManager.instance.bgmAudioSource.clip.name == this.sceneMusics[0].audioClip.name)
            {
                return;
            }

            BGMManager.instance.PlayMusic(sceneMusics[0].audioClip, fadeBetweenScenes);

        }

    }

}
