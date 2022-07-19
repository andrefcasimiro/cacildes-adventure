using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_PlayMusic : EventBase
    {
        public AudioClip bgm;

        public bool playMapMusic;

        public override IEnumerator Dispatch()
        {
            yield return null;

            if (playMapMusic)
            {
                SceneSettings sceneSettings = FindObjectOfType<SceneSettings>(true);

                BGMManager.instance.PlayMusic(sceneSettings.sceneMusic);
            }
            else
            {
                BGMManager.instance.PlayMusic(bgm);
            }
        }
    }

}
