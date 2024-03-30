using System.Collections;
using System.Collections.Generic;
using UniDi;
using UnityEngine;

public class VirtualAudioListener : MonoBehaviour
{
    public AudioListener Synced3DListener { get; private set; }

    [SerializeField] private AudioListener audioListener;

    [Inject] private IInstantiator instantiator;

    private void Start()
    {
        var sync = instantiator.InstantiateComponent<Sync2DTo3D>(audioListener.gameObject);
        sync.SetSyncedTransform(sync.transform);
        sync.StartTrackingTransform2D(transform);
    }
}
