using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Synced3DListener : MonoBehaviour
{
    [Header("Dependencies")] 
    [SerializeField] private Transform normalAudioListener;
    public Sync2DTo3D Sync2DTo3D => GetComponent<Sync2DTo3D>();

    public void SetupAndTrack(Transform trackedTransform)
    {
        Sync2DTo3D.SetSyncedTransform(normalAudioListener);
        Sync2DTo3D.transform.localPosition = Vector3.zero;
        Sync2DTo3D.StartTrackingTransform2D(trackedTransform);
    }
}
