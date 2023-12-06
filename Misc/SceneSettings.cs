using UnityEngine;
using System.Collections;
using AF.Music;
using TigerForge;
using AF.Events;
using UnityEngine.Events;

namespace AF
{
    public class SceneSettings : MonoBehaviour
    {
        [Header("Components")]
        public BGMManager bgmManager;
        public CursorManager cursorManager;
        public PlayerManager playerManager;
        public TeleportManager teleportManager;

        [Header("Music")]
        public AudioClip dayMusic;
        public AudioClip nightMusic;

        public AudioClip dayAmbience;
        public AudioClip nightAmbience;

        public AudioClip battleMusic;

        [Header("Map")]
        public bool isInterior;


        [Header("Tutorial")]
        public DestroyableParticle respawnFx;

        [Header("Systems")]
        public WorldSettings worldSettings;

        [Header("Events")]
        public UnityEvent onSceneStart;

        void Awake()
        {
            onSceneStart?.Invoke();

            StartCoroutine(SpawnPlayer());
        }

        private void Start()
        {

            OnHourChanged();

            EventManager.StartListening(EventMessages.ON_HOUR_CHANGED, OnHourChanged);
        }

        IEnumerator SpawnPlayer()
        {
            yield return null;
            teleportManager.SpawnPlayer(playerManager.gameObject);
        }

        public void HandleSceneSound()
        {
            EvaluateMusic();
            EvaluateAmbience();
        }

        /// <summary>
        /// Evaluate and control the music based on time of day.
        /// </summary>
        void EvaluateMusic()
        {
            if (dayMusic == null && nightMusic == null)
            {
                // Stop the music playback if there are no available tracks.
                bgmManager.StopMusic();
                return;
            }

            if (dayMusic != null && CanPlayDaySfx(dayMusic))
            {
                if (IsPlayingSameMusicTrack(dayMusic.name) == false)
                {
                    bgmManager.PlayMusic(dayMusic);
                }
            }
            else if (nightMusic != null && CanPlayNightSfx(nightMusic))
            {
                if (IsPlayingSameMusicTrack(nightMusic.name) == false)
                {
                    bgmManager.PlayMusic(nightMusic);
                }
            }
        }

        void EvaluateAmbience()
        {
            if (nightAmbience == null && dayAmbience == null)
            {
                bgmManager.StopAmbience();
                return;
            }

            if (dayAmbience != null && CanPlayDaySfx(dayAmbience))
            {
                if (IsPlayingSameAmbienceTrack(dayAmbience.name) == false)
                {
                    bgmManager.PlayAmbience(dayAmbience);
                }
            }
            else if (nightAmbience != null && CanPlayNightSfx(nightAmbience))
            {
                if (IsPlayingSameAmbienceTrack(nightAmbience.name) == false)
                {
                    bgmManager.PlayAmbience(nightAmbience);
                }
            }
        }

        bool IsPlayingSameMusicTrack(string musicClipName)
        {
            return bgmManager.bgmAudioSource.clip != null && bgmManager.bgmAudioSource.clip.name == musicClipName;
        }

        bool IsPlayingSameAmbienceTrack(string musicClipName)
        {
            return bgmManager.ambienceAudioSource.clip != null && bgmManager.ambienceAudioSource.clip.name == musicClipName;
        }

        bool IsNightTime()
        {
            return worldSettings.timeOfDay >= 20 && worldSettings.timeOfDay <= 24 || worldSettings.timeOfDay >= 0 && worldSettings.timeOfDay < 6;
        }

        bool CanPlayNightSfx(AudioClip clip)
        {
            return IsNightTime() && clip != null;
        }

        bool CanPlayDaySfx(AudioClip clip)
        {
            return !IsNightTime() && clip != null;
        }

        public void OnHourChanged()
        {
            HandleSceneSound();
        }

    }

}
