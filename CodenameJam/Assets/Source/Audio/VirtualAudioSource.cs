using System.Collections;
using System.Collections.Generic;
using UniDi;
using UnityEngine;

public class VirtualAudioSource : MonoBehaviour
{
    public Synced3DAudio Synced3DAudio { get; private set; }
    
    [SerializeField] private Synced3DAudio virtualAudioSourcePrefab;

    [Inject] private IInstantiator instantiator;

    private void Start()
    {
        Synced3DAudio = instantiator.InstantiatePrefabForComponent<Synced3DAudio>(virtualAudioSourcePrefab, transform);
        
        Synced3DAudio.Sync2DTo3D.transform.localPosition = Vector3.zero;
        Synced3DAudio.Sync2DTo3D.StartTrackingTransform2D(transform);
    }
}
