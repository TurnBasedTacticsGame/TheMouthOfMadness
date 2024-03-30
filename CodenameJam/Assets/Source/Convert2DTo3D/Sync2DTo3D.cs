using System;
using Source.Convert2DTo3D;
using UniDi;
using UnityEngine;

public class Sync2DTo3D : MonoBehaviour
{
    public Transform TrackedTransform2D => trackedTransform2D;

    [Header("Dependencies")] 
    [SerializeField] private Transform syncedTransform;
    
    [Inject] private Convert2DTo3D world3D;

    private Transform trackedTransform2D;
    private string originalName;
    
    private void Start()
    {
        originalName = syncedTransform.name;
        syncedTransform.SetParent(world3D.transform);
    }

    public void StartTrackingTransform2D(Transform transformToTrack)
    {
        trackedTransform2D = transformToTrack;
        syncedTransform.name = originalName + "Synced3D-" + trackedTransform2D.name;
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
