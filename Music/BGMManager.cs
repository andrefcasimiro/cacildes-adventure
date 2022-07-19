using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF
{
    [RequireComponent(typeof(AudioSource))]
    public class BGMManager : MonoBehaviour
    {
        AudioSource audioSource => GetComponent<AudioSource>();

        [HideInInspector]
        public AudioClip currentMusic;
        [HideInInspector]
        public AudioClip previousMusic;

        public static BGMManager instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }
        }

        private void Start()
        {
            SceneSettings sceneSettings = FindObjectOfType<SceneSettings>();

            if (sceneSettings != null && sceneSettings.sceneMusic != null)
            {
                PlayMusic(sceneSettings.sceneMusic);
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void PlayMusic(AudioClip musicToPlay)
        {
            this.previousMusic = this.currentMusic;
            this.currentMusic = musicToPlay;

            this.audioSource.clip = this.currentMusic;
            this.audioSource.Play();
        }

        public void PlayPreviousMusic()
        {
            AudioClip musicToPlay = this.previousMusic;

            this.PlayMusic(musicToPlay);
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneSettings sceneSettings = FindObjectOfType<SceneSettings>();

            if (sceneSettings != null && sceneSettings.sceneMusic != null && sceneSettings.sceneMusic.name != this.currentMusic.name)
            {
                PlayMusic(sceneSettings.sceneMusic);
            }
        }
    }
}