using System;
using Source.Players;
using UniDi;
using UnityEngine;

public class PickupLogCollectable : MonoBehaviour
{
    public event Action OnCollect;
    
    [Header("Dependencies")]
    [SerializeField] private TextData data;
    [SerializeField] private AudioSource audioSource;

    [Inject] private PickupLogUI pickupLogUI;

    private bool pickedUp;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (pickedUp || !(col.attachedRigidbody && col.attachedRigidbody.TryGetComponent(out Player _)))
        {
            return;
        }

        pickupLogUI.StartOpening(data);
        audioSource.Play();
        OnCollect?.Invoke();
        pickedUp = true;
        
        Destroy(gameObject);
    }
}
