using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class SceneMusicChanger : MonoBehaviour
    {
        public AudioClip musicToPlay;
        public bool playSceneMusic;

        SceneSettings sceneSettings;

        private void Start()
        {
            sceneSettings = FindObjectOfType<SceneSettings>(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (playSceneMusic)
                {
                    sceneSettings.HandleSceneSound();
                }
                else
                {
                    BGMManager.instance.PlayMusic(musicToPlay);
                }
            }
        }
    }

}
