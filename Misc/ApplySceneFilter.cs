using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        public void OnGameLoaded(GameData gameData)
        {
            Evaluate();
        }

        private void Awake()
        {
            sceneSettings = FindObjectOfType<SceneSettings>(true);
        }

        private void Start()
        {
            Evaluate();
        }

        void Evaluate()
        {

            bool switchValue = SwitchManager.instance.GetSwitchCurrentValue(switchEntry);
            foreach (var vol in FindObjectsOfType<PostProcessVolume>(true))
            {
                vol.profile = switchValue ? profileToApplyOnSwitchTrue : profileToApplyOnSwitchFalse;
            }

            if (switchValue)
            {
                sceneSettings.dayMusic = dayMusicIfSwitchTrue;
                sceneSettings.nightMusic = nightMusicIfSwitchTrue;
            }
            else
            {
                sceneSettings.dayMusic = dayMusicIfSwitchFalse;
                sceneSettings.nightMusic = nightMusicIfSwitchFalse;
            }

            sceneSettings.HandleSceneSound();
        }
    }

}
