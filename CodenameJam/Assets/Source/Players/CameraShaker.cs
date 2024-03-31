using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UniDi;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    [Inject] private CinemachineBrain cinemachineBrain;

    private CinemachineVirtualCamera virtualCamera;
    private float shakeTimer;

    private void Update()
    {
        if (virtualCamera == null)
            return;
        
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.unscaledDeltaTime;

            if (shakeTimer <= 0)
            {
                var noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                noise.m_AmplitudeGain = 0;
            }
        }
    }

    public void Shake(float intensity, float time)
    {
        virtualCamera = cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        var noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = intensity;
        shakeTimer = time;
    }
}
