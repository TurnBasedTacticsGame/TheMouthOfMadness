using System;
using System.Collections;
using System.Collections.Generic;
using Source.Players;
using UniDi;
using UnityEngine;

public class Bats : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject batEffectObject;
    
    [SerializeField] private float distance = 2;
    
    [Inject] private Player player;

    private void Update()
    {
        var directionTo = transform.position - player.transform.position;
        if (Vector3.Distance(player.transform.position, transform.position) < 2f
            && Vector3.Angle(player.transform.up, directionTo) < 30f)
        {
            batEffectObject.SetActive(true);
            batEffectObject.transform.parent = null;
            
            audioSource.Play();
            Destroy(gameObject);
        }
    }
}
