using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAudioPlayer : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private AudioSource audioSource;
    
    [Header("Configuration")]
    [SerializeField] private List<AudioClip> clips;

    public void PlayRandomOnce()
    {
        if (clips.Count == 0) return;
        
        var index = Random.Range(0, clips.Count);
        audioSource.clip = clips[index];
    }
}
