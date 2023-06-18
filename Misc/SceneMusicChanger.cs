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
                else if (BGMManager.instance.IsPlayingMusicClip(musicToPlay.name) == false)
                {
                    BGMManager.instance.PlayMusic(musicToPlay);
                }
            }
        }
    }

}
