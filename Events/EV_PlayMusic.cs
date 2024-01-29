﻿using System.Collections;
using AF.Music;
using UnityEngine;

namespace AF
{

    public class EV_PlayMusic : EventBase
    {
        [Header("Components")]
        public SceneSettings sceneSettings;
        public BGMManager bgmManager;
        public AudioClip bgm;

        public bool playMapMusic;

        public override IEnumerator Dispatch()
        {
            yield return null;

            if (playMapMusic)
            {
                sceneSettings.HandleSceneSound(true);
            }
            else if (bgm != null)
            {
                bgmManager.PlayMusic(bgm);
            }
            else
            {
                bgmManager.StopMusic();
            }
        }
    }

}
