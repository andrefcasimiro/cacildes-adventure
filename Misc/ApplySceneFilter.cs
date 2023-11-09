using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;

namespace AF
{
    public class ApplySceneFilter : MonoBehaviour, ISaveable
    {

        public SwitchEntry switchEntry;
        public PostProcessProfile profileToApplyOnSwitchFalse;
        public PostProcessProfile profileToApplyOnSwitchTrue;

        public AudioClip dayMusicIfSwitchTrue;
        public AudioClip dayMusicIfSwitchFalse;
        public AudioClip nightMusicIfSwitchTrue;
        public AudioClip nightMusicIfSwitchFalse;

        SceneSettings sceneSettings;

        AudioClip fallbackDefaultDaySceneMusic;
        AudioClip fallbackDefaultNightSceneMusic;

        [Header("Secondary Switch - Edge cases like West Bridge map")]
        public SwitchEntry switch2;
        public bool requiredSwitch2Value;
        public UnityEvent onNotMeetingRequirements;

        public void OnGameLoaded(object gameData)
        {

            Evaluate();
        }

        private void Awake()
        {
            sceneSettings = FindAnyObjectByType<SceneSettings>(FindObjectsInactive.Include);

            fallbackDefaultDaySceneMusic = sceneSettings.dayMusic;
            fallbackDefaultNightSceneMusic = sceneSettings.nightMusic;
        }

        private void Start()
        {
            Evaluate();
        }

        public void Evaluate()
        {
            BGMManager.instance.StopMusicImmediately();

            bool switchValue = SwitchManager.instance.GetSwitchCurrentValue(switchEntry);
            foreach (var vol in FindObjectsOfType<PostProcessVolume>(true))
            {
                vol.profile = switchValue ? profileToApplyOnSwitchTrue : profileToApplyOnSwitchFalse;
            }

            if (switch2 != null && SwitchManager.instance.GetSwitchCurrentValue(switch2) != requiredSwitch2Value)
            {
                if (onNotMeetingRequirements != null)
                {
                    onNotMeetingRequirements.Invoke();
                }
                return;
            }

            if (switchValue)
            {
                sceneSettings.dayMusic = dayMusicIfSwitchTrue;
                sceneSettings.nightMusic = nightMusicIfSwitchTrue;
            }
            else
            {
                sceneSettings.dayMusic = dayMusicIfSwitchFalse != null ? dayMusicIfSwitchFalse : fallbackDefaultDaySceneMusic;
                sceneSettings.nightMusic = nightMusicIfSwitchFalse != null ? nightMusicIfSwitchFalse : fallbackDefaultNightSceneMusic;
            }

            sceneSettings.HandleSceneSound();
        }
    }

}
