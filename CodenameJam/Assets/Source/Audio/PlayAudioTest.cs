using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioTest : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private VirtualAudioSource virtualAudioSource;
    
    [ContextMenu("Play clip at 3D location")]
    private void Test()
    {
        virtualAudioSource.Synced3DAudio.AudioSource.clip = clip;
        virtualAudioSource.Synced3DAudio.AudioSource.Play();
    }
}
