using AF.Music;
using UnityEngine;

namespace AF
{
    public class SceneMusicChanger : MonoBehaviour
    {
        [Header("Components")]
        public BGMManager bgmManager;
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
                    sceneSettings.HandleSceneSound(true);
                }
                else if (bgmManager.IsPlayingMusicClip(musicToPlay.name) == false)
                {
                    bgmManager.PlayMusic(musicToPlay);
                }
            }
        }
    }

}
