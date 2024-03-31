using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Light2DScaler : MonoBehaviour
{
    [SerializeField] private Light2D light2D;
    
    private float startingLightIntensity = -1;
    private float startingLightFalloffIntensity = -1;

    private void Start()
    {
        startingLightIntensity = light2D.intensity;
        startingLightFalloffIntensity = light2D.falloffIntensity;
    }

    public void SetPercentage(float percentage)
    {
        light2D.intensity = Mathf.Lerp(0, startingLightIntensity, percentage);
        light2D.falloffIntensity = Mathf.Lerp(1, startingLightFalloffIntensity, percentage);
    }
}
