using UniDi;
using UnityEngine;

public class VirtualAudioSource : MonoBehaviour
{
    public AudioSource AudioSource { get; private set; }
    
    [SerializeField] private AudioSource audioSource;

    [Inject] private IInstantiator instantiator;

    private void Start()
    {
        var sync = instantiator.InstantiateComponent<Sync2DTo3D>(audioSource.gameObject);
        sync.SetSyncedTransform(sync.transform);
        sync.StartTrackingTransform2D(transform);
    }
}
