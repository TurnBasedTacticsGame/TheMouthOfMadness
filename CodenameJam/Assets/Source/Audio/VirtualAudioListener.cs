using System.Collections;
using System.Collections.Generic;
using UniDi;
using UnityEngine;

public class VirtualAudioListener : MonoBehaviour
{
    public Synced3DListener Synced3DListener { get; private set; }

    [SerializeField] private Synced3DListener virtualAudioListenerPrefab;

    [Inject] private IInstantiator instantiator;

    private void Start()
    {
        Synced3DListener = instantiator.InstantiatePrefabForComponent<Synced3DListener>(virtualAudioListenerPrefab, transform);
        Synced3DListener.SetupAndTrack(transform);
    }
}
