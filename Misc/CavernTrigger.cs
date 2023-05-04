using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class CavernTrigger : MonoBehaviour, ISaveable
    {
        public AudioClip cavernMusic;
        public AudioClip cavernAmbience;

        SceneSettings sceneSettings;
        DayNightManager dayNightManager;

        AudioClip dayMusic;
        AudioClip nightMusic;
        AudioClip dayAmbience;
        AudioClip nightAmbience;

        public Gradient AmbientColor;
        public Gradient DirectionalColor;
        public Gradient FogColor;

        public Gradient sceneAmbientColor;
        public Gradient sceneDirectionalColor;
        public Gradient sceneFogColor;

        public UnityEvent onCavernEnterEvent;
        public UnityEvent onCavernExitEvent;

        private void Awake()
        {
             sceneSettings = FindObjectOfType<SceneSettings>(true);
             dayNightManager = FindObjectOfType<DayNightManager>(true);
        }

        private void Start()
        {
            dayMusic = sceneSettings.dayMusic;
            nightMusic = sceneSettings.nightMusic;
            dayAmbience = sceneSettings.dayAmbience;
            nightAmbience = sceneSettings.nightAmbience;
            sceneAmbientColor = dayNightManager.AmbientColor;
            sceneDirectionalColor = dayNightManager.DirectionalColor;
            sceneFogColor = dayNightManager.FogColor;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {

                PlayDungeon();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {

                PlayMap();
            }
        }

        void PlayDungeon()
        {
            sceneSettings.dayMusic = cavernMusic;
            sceneSettings.nightMusic = cavernMusic;

            sceneSettings.dayAmbience = cavernAmbience;
            sceneSettings.nightAmbience = cavernAmbience;

            sceneSettings.HandleSceneSound();

            dayNightManager.AmbientColor = AmbientColor;
            dayNightManager.DirectionalColor = DirectionalColor;
            dayNightManager.FogColor = FogColor;

            if (onCavernEnterEvent != null)
            {
                onCavernEnterEvent.Invoke();
            }
        }

        void PlayMap()
        {
            sceneSettings.dayMusic = dayMusic;
            sceneSettings.nightMusic = nightMusic;
            sceneSettings.dayAmbience = dayAmbience;
            sceneSettings.nightAmbience = nightAmbience;

            sceneSettings.HandleSceneSound();

            dayNightManager.AmbientColor = sceneAmbientColor;
            dayNightManager.DirectionalColor = sceneDirectionalColor;
            dayNightManager.FogColor = sceneFogColor;

            if (onCavernExitEvent != null)
            {
                onCavernExitEvent.Invoke();
            }
        }

        public void OnGameLoaded(GameData gameData)
        {
            if (IsPointWithinCollider(GetComponent<Collider>(), GameObject.FindWithTag("Player").transform.position))
            {
                PlayDungeon();
            }
            else
            {
                PlayMap();
            }
        }

        bool IsPointWithinCollider(Collider collider, Vector3 point)
        {
            return (collider.ClosestPoint(point) - point).sqrMagnitude < Mathf.Epsilon * Mathf.Epsilon;
        }
    }

}
