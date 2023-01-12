using Unity.Services.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Services.Analytics;
using Unity.Services.Core.Environments;

namespace AF
{
    public class Analytics : MonoBehaviour
    {
        async void Awake()
        {
            try
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
            }
        }



     }

}
