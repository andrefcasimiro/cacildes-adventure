using UnityEngine;
using System.Collections;

namespace AF
{
    public class SceneSettings: MonoBehaviour, IClockListener
    {
        [Header("Music")]
        public AudioClip dayMusic;
        public AudioClip nightMusic;

        public AudioClip dayAmbience;
        public AudioClip nightAmbience;

        public AudioClip battleMusic;

        [Header("Map")]
        public bool isInterior;

        [Header("Debug")]
        public bool enemiesIgnorePlayer;

        public void Start()
        {
            HandleSceneSound();

            if (enemiesIgnorePlayer)
            {
                var allEnemies = FindObjectsOfType<EnemySightController>(true);
                foreach (var enemy in allEnemies)
                {
                    enemy.ignorePlayer = true;
                }
            }
        }

        public void HandleSceneSound()
        {
            EvaluateMusic();
            EvaluateAmbience();
        }

        void EvaluateMusic()
        {
            if (nightMusic == null && dayMusic == null)
            {
                BGMManager.instance.StopMusic();
                return;
            }

            // Play day only
            if (nightMusic == null)
            {
                if (IsPlayingSameMusic(dayMusic.name))
                {
                    return;
                }

                BGMManager.instance.PlayMusic(dayMusic);
                return;
            }

            if (Player.instance.timeOfDay >= 20 && Player.instance.timeOfDay <= 24 || Player.instance.timeOfDay >= 0 && Player.instance.timeOfDay < 6)
            {
                if (IsPlayingSameMusic(nightMusic.name))
                {
                    return;
                }

                BGMManager.instance.PlayMusic(nightMusic);
            }
            else
            {
                if (IsPlayingSameMusic(dayMusic.name))
                {
                    return;
                }

                BGMManager.instance.PlayMusic(dayMusic);
            }

        }

        bool IsPlayingSameMusic(string musicClipName)
        {
            return BGMManager.instance.bgmAudioSource.clip != null && BGMManager.instance.bgmAudioSource.clip.name == musicClipName;
        }

        void EvaluateAmbience()
        {
            if (nightAmbience == null && dayAmbience == null)
            {
                BGMManager.instance.StopAmbience();
                return;
            }

            // Play day only
            if (nightAmbience == null)
            {
                if (IsPlayingSameAmbience(dayAmbience.name))
                {
                    return;
                }

                BGMManager.instance.PlayAmbience(dayAmbience);
                return;
            }

            if (Player.instance.timeOfDay >= 20 && Player.instance.timeOfDay <= 24 || Player.instance.timeOfDay >= 0 && Player.instance.timeOfDay < 6)
            {
                if (IsPlayingSameAmbience(nightAmbience.name))
                {
                    return;
                }

                BGMManager.instance.PlayAmbience(nightAmbience);
            }
            else
            {
                if (IsPlayingSameAmbience(dayAmbience.name))
                {
                    return;
                }

                BGMManager.instance.PlayAmbience(dayAmbience);
            }

        }

        bool IsPlayingSameAmbience(string musicClipName)
        {
            return BGMManager.instance.ambienceAudioSource.clip != null && BGMManager.instance.ambienceAudioSource.clip.name == musicClipName;
        }

        public void OnHourChanged()
        {
            HandleSceneSound();
        }
    }

}
