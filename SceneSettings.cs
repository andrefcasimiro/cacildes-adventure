using UnityEngine;
using System.Collections;

namespace AF
{
    public class SceneSettings: MonoBehaviour
    {
        [Header("Music")]
        public AudioClip sceneMusic;
        public AudioClip sceneAmbience;

        [Header("Map")]
        public bool isInterior;
        
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
            if (sceneMusic == null)
            {
                return;
            }

            if (BGMManager.instance.bgmAudioSource.clip != null && BGMManager.instance.bgmAudioSource.clip.name == sceneMusic.name)
            {
                return;
            }

            BGMManager.instance.PlayMusic(sceneMusic);

        }

    }

}
