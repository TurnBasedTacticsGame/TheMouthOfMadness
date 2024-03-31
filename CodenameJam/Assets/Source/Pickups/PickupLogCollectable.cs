using System;
using System.Collections;
using System.Collections.Generic;
using UniDi;
using UnityEngine;

public class PickupLogCollectable : MonoBehaviour
{
    [SerializeField] private PickupLogData data;

    [Inject] private PickupLogUI pickupLogUI;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        pickupLogUI.Open(data);
        Destroy(gameObject);
    }
}
