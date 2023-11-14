using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class EV_Sleep : EventBase
    {
        [Header("Systems")]
        public WorldSettings worldSettings;

        public int targetHour = 0;

        public bool fadeToBlack = true;

        public override IEnumerator Dispatch()
        {
            bool isInteriorOriginal = FindObjectOfType<SceneSettings>(true).isInterior;

            FindObjectOfType<SceneSettings>(true).isInterior = false;
            FindObjectOfType<DayNightManager>(true).tick = true;
            var originalDaySpeed = worldSettings.daySpeed;

            if (fadeToBlack)
            {
                FindObjectOfType<UIBlackCanvas>(true).gameObject.SetActive(true);
            }
            FindObjectOfType<UIDocumentDialogueWindow>(true).gameObject.SetActive(false);

            yield return null;

            worldSettings.daySpeed = 2;

            yield return new WaitUntil(() => Mathf.Floor(worldSettings.timeOfDay) == targetHour);

            worldSettings.daySpeed = originalDaySpeed;

            FindObjectOfType<DayNightManager>(true).tick = FindObjectOfType<DayNightManager>(true).TimePassageAllowed();
            FindObjectOfType<SceneSettings>(true).isInterior = isInteriorOriginal;

            if (fadeToBlack)
            {

                FindObjectOfType<UIBlackCanvas>(true).StartFade();

                yield return new WaitForSeconds(FindObjectOfType<UIBlackCanvas>(true).fadeTime);
            }

            FindObjectOfType<PlayerComponentManager>(true).CurePlayer();
        }

    }
}
