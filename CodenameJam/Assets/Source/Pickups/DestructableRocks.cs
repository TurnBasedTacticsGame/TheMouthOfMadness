using System;
using System.Collections;
using System.Collections.Generic;
using UniDi;
using UnityEngine;

public class DestructableRocks : MonoBehaviour
{ 
    [Header("Dependencies")]
    [SerializeField] private AudioSource audioSource;
    
    [Header("Configuration")]
    [SerializeField] private List<PickupLogCollectable> pickupLogsRequiredToOpen;

    [Inject] private PickupLogUI pickupLogUI;
    
    private int crackCount;
    
    private void OnEnable()
    {
        foreach (var pickupLogCollectable in pickupLogsRequiredToOpen)
        {
            pickupLogCollectable.OnCollect += CrackFurther;
        }

        pickupLogUI.OnPickupTextClose += TryCrackCompletely;
    }

    private void CrackFurther()
    {
        crackCount++;
    }

    private void TryCrackCompletely()
    {
        if (!this)
        {
            return;
        }
        
        if (crackCount >= pickupLogsRequiredToOpen.Count)
        {
            audioSource.Play();
            Destroy(gameObject);
        }
    }
}
