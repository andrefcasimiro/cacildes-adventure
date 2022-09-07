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
                sceneSettings.PlaySceneMusic();
            }
            else
            {
                SceneSettings sceneSettings = FindObjectOfType<SceneSettings>(true);
                sceneSettings.StopAllCoroutines();

                BGMManager.instance.PlayMusic(bgm);
            }
        }
    }

}
