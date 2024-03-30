using System;
using Source.Convert2DTo3D;
using UniDi;
using UnityEngine;

public class Sync2DTo3D : MonoBehaviour
{
    public Transform TrackedTransform2D => trackedTransform2D;

    [Inject] private Convert2DTo3D world3D;
    
    private Transform syncedTransform;
    private Transform trackedTransform2D;
    private string originalName;

    public void StartTrackingTransform2D(Transform transformToTrack)
    {
        trackedTransform2D = transformToTrack;
        syncedTransform.name = originalName + "Synced3D-" + trackedTransform2D.name;
    }

    public void SetSyncedTransform(Transform transformToSync)
    {
        if (syncedTransform != null)
        {
            syncedTransform.name = originalName;
        }
        
        syncedTransform = transformToSync;
        originalName = syncedTransform.name;
        syncedTransform.SetParent(world3D.transform);
    }

    private void Update()
    {
        if (trackedTransform2D != null)
        {
            var positionXY = trackedTransform2D.position;
            syncedTransform.localPosition = positionXY;
        }
    }
}
