using System.Collections.Generic;
using UnityEngine;
using System;

namespace AF
{
    public class Analytics : MonoBehaviour
    {
        async void Awake()
        {
            /*try
            {
                await UnityServices.InitializeAsync();

              var options = new InitializationOptions();
            options.SetEnvironmentName("production");

                List<string> consentIdentifiers = await AnalyticsService.Instance.CheckForRequiredConsents();
                Debug.Log(consentIdentifiers.Count);

            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }*/
        }

        public void OptOut()
        {
            //AnalyticsService.Instance.OptOut();
        }

       
        public void TrackAnalyticsEvent(string message)
        {
            /*if (PlayerPrefs.GetInt("dontAllowAds") == 1)
            {
                return;
            }

            AnalyticsService.Instance.CustomData("game_event",
                    new Dictionary<string, object>()
                    {
                                { "message", message },
                    }
                );*/
        }



     }

}
