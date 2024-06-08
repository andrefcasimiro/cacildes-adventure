using System.Collections;
using AF.Music;
using UnityEngine;

namespace AF
{

    public class EV_PlayMusic : EventBase
    {
        [Header("Components")]
        SceneSettings _sceneSettings;
        BGMManager _bgmManager;
        public AudioClip bgm;

        public bool playMapMusic;

        public override IEnumerator Dispatch()
        {
            yield return null;

            if (playMapMusic)
            {
                GetSceneSettings().HandleSceneSound(true);
            }
            else if (bgm != null)
            {
                GetBGMManager().PlayMusic(bgm);
            }
            else
            {
                GetBGMManager().StopMusic();
            }
        }
        BGMManager GetBGMManager()
        {
            if (_bgmManager == null)
            {
                _bgmManager = FindAnyObjectByType<BGMManager>(FindObjectsInactive.Include);
            }

            return _bgmManager;
        }
        SceneSettings GetSceneSettings()
        {
            if (_sceneSettings == null)
            {
                _sceneSettings = FindAnyObjectByType<SceneSettings>(FindObjectsInactive.Include);
            }

            return _sceneSettings;
        }

    }

}
