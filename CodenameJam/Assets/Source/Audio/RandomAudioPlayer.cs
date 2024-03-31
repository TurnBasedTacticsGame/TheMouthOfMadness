using System;
using System.Collections;
using System.Collections.Generic;
using UniDi;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomAudioPlayer : MonoBehaviour
{
    [Tooltip("Leave as null if using Player's oneshot source")]
    [SerializeField] private AudioSource optionalAudioSource;

    [Inject] private AudioSource playerAudioSource;

    private AudioSource currentAudioSource;
    private AudioClipData audioClipData;
    private bool playingUntilStopped;
    private float timer = 0;

    private void Start()
    {
        currentAudioSource = optionalAudioSource != null ? optionalAudioSource : playerAudioSource;
    }

    private void Update()
    {
        if (!playingUntilStopped) return;

        timer += Time.deltaTime;

        if (timer >= audioClipData.AverageTimePerClip)
        {
            timer -= audioClipData.AverageTimePerClip;
            PlayRandomOnce(audioClipData);
        }
    }

    public void PlayRandomOnce(AudioClipData data)
    {
        if (data.AudioClips.Length == 0) return;
        
        var index = Random.Range(0, data.AudioClips.Length);
        currentAudioSource.clip = data.AudioClips[index];
        currentAudioSource.Play();
    }

    public void StartPlayingRandom(AudioClipData data)
    {
        audioClipData = data;
        playingUntilStopped = true;
    }

    public void StopPlayingRandom()
    {
        playingUntilStopped = false;
    }
}
