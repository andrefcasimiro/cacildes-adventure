using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSource : MonoBehaviour
{
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.1f;

    public float speed = 0.2f;

    Light light => GetComponent<Light>();

    bool lightOn = false;

    // Update is called once per frame
    void Update()
    {
        if (lightOn)
        {
            light.intensity -= speed * Time.deltaTime;
        }
        else
        {
            light.intensity += speed * Time.deltaTime;
        }

        if (light.intensity >= maxIntensity)
        {
            lightOn = true;
        }
        else if (light.intensity <= minIntensity)
        {
            lightOn = false;
        }
    }
}
