using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.InputSystem;

namespace AF
{
    public class SceneSettings : MonoBehaviour, IClockListener
    {
        CursorManager cursorManager;

        [Header("Music")]
        public AudioClip dayMusic;
        public AudioClip nightMusic;

        public AudioClip dayAmbience;
        public AudioClip nightAmbience;

        public AudioClip battleMusic;

        [Header("Map")]
        public bool isInterior;
        public bool isTitleScreen = false;

        [Header("Debug")]
        public bool enemiesIgnorePlayer = false;

        [Header("Tutorial")]
        public bool isSceneTutorial = false;
        public DestroyableParticle respawnFx;

        [Header("Colliseum")]
        public bool isColliseum = false;
        [Header("Systems")]
        public WorldSettings worldSettings;

        PlayerComponentManager playerComponentManager;

        void Awake()
        {
            cursorManager = FindAnyObjectByType<CursorManager>(FindObjectsInactive.Include);
            playerComponentManager = FindAnyObjectByType<PlayerComponentManager>(FindObjectsInactive.Include);

            StartCoroutine(SpawnPlayer());

            HandleSceneSound();
        }

        IEnumerator SpawnPlayer()
        {
            yield return null;

            TeleportManager.instance.SpawnPlayer(playerComponentManager.gameObject);
        }

        public void Start()
        {

            if (enemiesIgnorePlayer)
            {
                var allEnemies = FindObjectsOfType<EnemyManager>(true);
                foreach (var enemy in allEnemies)
                {
                    if (enemy.enemySightController != null)
                    {
                        enemy.enemySightController.ignorePlayer = true;
                    }
                }
            }



            GamePreferences.instance.UpdateGraphics();
        }

        #region Scene Music & Ambience
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
                BGMManager.instance.StopMusic();
                return;
            }

            if (dayMusic != null && CanPlayDaySfx(dayMusic))
            {
                if (IsPlayingSameMusicTrack(dayMusic.name) == false)
                {
                    BGMManager.instance.PlayMusic(dayMusic);
                }
            }
            else if (nightMusic != null && CanPlayNightSfx(nightMusic))
            {
                if (IsPlayingSameMusicTrack(nightMusic.name) == false)
                {
                    BGMManager.instance.PlayMusic(nightMusic);
                }
            }
        }

        void EvaluateAmbience()
        {
            if (nightAmbience == null && dayAmbience == null)
            {
                BGMManager.instance.StopAmbience();
                return;
            }

            if (dayAmbience != null && CanPlayDaySfx(dayAmbience))
            {
                if (IsPlayingSameAmbienceTrack(dayAmbience.name) == false)
                {
                    BGMManager.instance.PlayAmbience(dayAmbience);
                }
            }
            else if (nightAmbience != null && CanPlayNightSfx(nightAmbience))
            {
                if (IsPlayingSameAmbienceTrack(nightAmbience.name) == false)
                {
                    BGMManager.instance.PlayAmbience(nightAmbience);
                }
            }
        }

        bool IsPlayingSameMusicTrack(string musicClipName)
        {
            return BGMManager.instance.bgmAudioSource.clip != null && BGMManager.instance.bgmAudioSource.clip.name == musicClipName;
        }

        bool IsPlayingSameAmbienceTrack(string musicClipName)
        {
            return BGMManager.instance.ambienceAudioSource.clip != null && BGMManager.instance.ambienceAudioSource.clip.name == musicClipName;
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
        #endregion

        #region Tutorial
        public void RestartTutorialFromCheckpoint(Vector3 playerPosition)
        {
            // Cure player
            playerComponentManager.CurePlayer();

            // Find all active enemies in scene
            var allEnemiesInScene = FindObjectsOfType<EnemyManager>();
            foreach (var enemy in allEnemiesInScene)
            {
                if (enemy.enemyBossController != null)
                {
                    continue;
                }

                enemy.enabled = true;
                enemy.Revive();
            }

            // Attempt to teleport player
            var newPlayerPosition = Utils.GetNearestNavMeshPoint(playerPosition);

            playerComponentManager.UpdatePosition(newPlayerPosition, Quaternion.identity);
            Instantiate(respawnFx, newPlayerPosition, Quaternion.identity);
        }
        #endregion
    }

}
