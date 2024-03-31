using System;
using System.Collections;
using System.Collections.Generic;
using UniDi;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomAudioPlayer : MonoBehaviour
{
    [SerializeField] private VirtualAudioSource audioSourcePrefab;
    [SerializeField] private int SourceCount = 10;

    [Inject] private IInstantiator instantiator;

    private AudioClipData audioClipData;
    private bool playingUntilStopped;
    private float timer = 0;

    private int soundsPlayed;
    private List<VirtualAudioSource> audioSources = new();

    private void Start()
    {
        for (var i = 0; i < SourceCount; i++)
        {
            var source = instantiator.InstantiatePrefabForComponent<VirtualAudioSource>(audioSourcePrefab, transform);
            source.AudioSource.playOnAwake = false;
            source.AudioSource.Stop();

            audioSources.Add(source);
        }
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
        
        audioSources[soundsPlayed % audioSources.Count].AudioSource.clip = data.AudioClips[Random.Range(0, data.AudioClips.Length)];
        audioSources[soundsPlayed % audioSources.Count].AudioSource.Play();
        soundsPlayed++;
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
