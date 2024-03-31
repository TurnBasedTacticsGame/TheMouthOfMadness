using System.Collections;
using System.Collections.Generic;
using UniDi;
using UnityEngine;

public class RandomAudioPlayer : MonoBehaviour
{
    [Inject] private AudioSource playerAudioSource;
    
    public void PlayRandomOnce(AudioClipData data)
    {
        if (data.AudioClips.Length == 0) return;
        
        var index = Random.Range(0, data.AudioClips.Length);
        playerAudioSource.clip = data.AudioClips[index];
        playerAudioSource.Play();
    }
}
