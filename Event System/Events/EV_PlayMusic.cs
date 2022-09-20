using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_PlayMusic : EventBase
    {
        public AudioClip bgm;

        public bool playMapMusic;

        public float fadeAmountOfPreviousMusic = 0f;

        public override IEnumerator Dispatch()
        {
            yield return null;

            if (playMapMusic)
            {
                SceneSettings sceneSettings = FindObjectOfType<SceneSettings>(true);
                sceneSettings.PlaySceneMusic();
            }
            else
            {
                BGMManager.instance.PlayMusic(bgm, fadeAmountOfPreviousMusic);
            }
        }
    }

}
