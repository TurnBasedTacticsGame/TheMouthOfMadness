using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio Clip Data", menuName = "Audio Clip Data")]
public class AudioClipData : ScriptableObject
{
    public AudioClip[] AudioClips => audioClips;
    
    [SerializeField] private AudioClip[] audioClips;
}
