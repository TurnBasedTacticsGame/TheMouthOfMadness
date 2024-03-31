using SteamAudio;
using UniDi;
using UnityEngine;

public class VirtualAudioSource : MonoBehaviour
{
    public AudioSource AudioSource => audioSource;
    
    [SerializeField] private AudioSource audioSource;

    [Inject] private IInstantiator instantiator;
    [Inject] private SteamAudioProbeBatch probes;

    private void Start()
    {
        var sync = instantiator.InstantiateComponent<Sync2DTo3D>(audioSource.gameObject);
        sync.SetSyncedTransform(sync.transform);
        sync.StartTrackingTransform2D(transform);

        if (audioSource.TryGetComponent(out SteamAudioSource source))
        {
            source.pathingProbeBatch = probes;
        }
    }
}
