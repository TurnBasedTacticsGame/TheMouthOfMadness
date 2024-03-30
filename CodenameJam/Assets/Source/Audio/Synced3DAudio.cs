using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Sync2DTo3D), typeof(AudioSource))]
public class Synced3DAudio : MonoBehaviour
{
    public AudioSource AudioSource => GetComponent<AudioSource>();
    public Sync2DTo3D Sync2DTo3D => GetComponent<Sync2DTo3D>();
}
