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

    private int requiredLogsCount;
    private int crackCount;
    
    private void OnEnable()
    {
        foreach (var pickupLogCollectable in pickupLogsRequiredToOpen)
        {
            pickupLogCollectable.OnCollect += CrackFurther;
        }

        pickupLogUI.OnPickupTextClose += TryCrackCompletely;
    }
    
    private void OnDisable()
    {
        pickupLogUI.OnPickupTextClose -= TryCrackCompletely;
    }

    private void Start()
    {
        requiredLogsCount = pickupLogsRequiredToOpen.Count;
    }

    private void CrackFurther()
    {
        crackCount++;
    }

    private void TryCrackCompletely()
    {
        if (crackCount >= requiredLogsCount)
        {
            audioSource.Play();
            Destroy(gameObject);
        }
    }
}
