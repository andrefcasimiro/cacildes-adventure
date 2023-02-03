using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class EV_Sleep : EventBase
    {
        public int targetHour = 0;

        public bool fadeToBlack = true;

        public override IEnumerator Dispatch()
        {
            bool isInteriorOriginal = FindObjectOfType<SceneSettings>(true).isInterior;

            FindObjectOfType<SceneSettings>(true).isInterior = false;
            FindObjectOfType<DayNightManager>(true).tick = true;
            var originalDaySpeed = Player.instance.daySpeed;

            if (fadeToBlack)
            {
                FindObjectOfType<UIBlackCanvas>(true).gameObject.SetActive(true);
            }
            FindObjectOfType<UIDocumentDialogueWindow>(true).gameObject.SetActive(false);

            yield return null;

            Player.instance.daySpeed = 2;

            yield return new WaitUntil(() => Mathf.Floor(Player.instance.timeOfDay) == targetHour);

            Player.instance.daySpeed = originalDaySpeed;

            FindObjectOfType<DayNightManager>(true).tick = FindObjectOfType<DayNightManager>(true).TimePassageAllowed();
            FindObjectOfType<SceneSettings>(true).isInterior = isInteriorOriginal;

            if (fadeToBlack) {

                FindObjectOfType<UIBlackCanvas>(true).StartFade();

                yield return new WaitForSeconds(FindObjectOfType<UIBlackCanvas>(true).fadeTime);
            }

            FindObjectOfType<PlayerComponentManager>(true).CurePlayer();
        }

    }
}
