using UnityEngine;
using System.Collections;
using System.Linq;
using Mono.Cecil.Cil;

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
        public bool isTitleScreen = false;

        [Header("Debug")]
        public bool enemiesIgnorePlayer = false;

        [Header("Tutorial")]
        public bool isSceneTutorial = false;
        public DestroyableParticle respawnFx;

        PlayerComponentManager playerComponentManager => FindObjectOfType<PlayerComponentManager>(true);
 
        void Awake()
        {
            StartCoroutine(SpawnPlayer());
        }

        IEnumerator SpawnPlayer()
        {
            yield return null;

            TeleportManager.instance.SpawnPlayer(playerComponentManager.gameObject);
        }

        public void Start()
        {
            HandleSceneSound();

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

            if (isTitleScreen && Player.instance.hasShownTitleScreen == false)
            {
                Utils.ShowCursor();
            }
            else
            {
                Utils.HideCursor();
            }

            GamePreferences.instance.UpdateGraphics();
        }

        #region Scene Music & Ambience
        public void HandleSceneSound()
        {
            // Don't change map music if a combat is on going
            var allEnemies = FindObjectsOfType<EnemyManager>(true);
            if (allEnemies != null && allEnemies.Length > 0 && allEnemies.FirstOrDefault(x => x.enemyCombatController.IsInCombat()))
            {
                return;
            }

            EvaluateMusic(false);
            EvaluateMusic(true);
        }

        void EvaluateMusic(bool isAmbience)
        {
            var daySfx = dayMusic;
            var nightSfx = nightMusic;

            if (isAmbience)
            {
                daySfx = dayAmbience;
                nightSfx = nightAmbience;
            }

            if (nightSfx == null && daySfx == null)
            {
                if (isAmbience)
                {
                    BGMManager.instance.StopAmbience();
                }
                else
                {
                    BGMManager.instance.StopMusic();
                }

                return;
            }

            // If no night track is set, play day always
            if (nightSfx == null && daySfx != null)
            {
                if (IsPlayingSameTrack(daySfx.name, isAmbience))
                {
                    return;
                }

                if (isAmbience)
                {
                    BGMManager.instance.PlayAmbience(daySfx);
                }
                else
                {
                    BGMManager.instance.PlayMusic(daySfx);
                }

                return;
            }

            // If we have day and night track, decide which to play based on time of day
            if (CanPlayNightSfx(nightSfx))
            {
                if (IsPlayingSameTrack(nightSfx.name, isAmbience))
                {
                    return;
                }

                if (isAmbience)
                {
                    BGMManager.instance.PlayAmbience(nightSfx);
                }
                else
                {
                    BGMManager.instance.PlayMusic(nightSfx);
                }

                return;
            }

            if (CanPlayDaySfx(daySfx))
            {
                if (IsPlayingSameTrack(daySfx.name, isAmbience))
                {
                    return;
                }

                if (isAmbience)
                {
                    BGMManager.instance.PlayAmbience(daySfx);
                }
                else
                {
                    BGMManager.instance.PlayMusic(daySfx);
                }

                return;
            }
        }

        bool IsPlayingSameTrack(string musicClipName, bool isAmbience)
        {
            if (isAmbience)
            {
                return BGMManager.instance.ambienceAudioSource.clip != null && BGMManager.instance.ambienceAudioSource.clip.name == musicClipName;
            }

            return BGMManager.instance.bgmAudioSource.clip != null && BGMManager.instance.bgmAudioSource.clip.name == musicClipName;
        }

        bool IsNightTime()
        {
            return Player.instance.timeOfDay >= 20 && Player.instance.timeOfDay <= 24 || Player.instance.timeOfDay >= 0 && Player.instance.timeOfDay < 6;
        }

        bool CanPlayNightSfx(AudioClip clip)
        {
            return IsNightTime() && clip != null;
        }

        bool CanPlayDaySfx(AudioClip clip)
        {
            return IsNightTime() == false && clip != null;
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
