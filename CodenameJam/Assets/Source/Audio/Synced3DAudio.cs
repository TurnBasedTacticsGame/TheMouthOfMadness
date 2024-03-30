using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Sync2DTo3D))]
public class Synced3DAudio : MonoBehaviour
{
    [Header("Dependencies")] 
    [SerializeField] private Transform normalAudioSource;
    
    public AudioSource AudioSource => normalAudioSource.GetComponent<AudioSource>();
    public Sync2DTo3D Sync2DTo3D => GetComponent<Sync2DTo3D>();

    public void SetupAndTrack(Transform trackedTransform)
    {
        Sync2DTo3D.SetSyncedTransform(normalAudioSource);
        Sync2DTo3D.transform.localPosition = Vector3.zero;
        Sync2DTo3D.StartTrackingTransform2D(trackedTransform);
    }
}
