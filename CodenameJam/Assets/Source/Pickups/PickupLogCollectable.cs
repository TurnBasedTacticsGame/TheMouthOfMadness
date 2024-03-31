using System;
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
        if (pickedUp) return;

        pickupLogUI.StartOpening(data);
        audioSource.Play();
        OnCollect?.Invoke();
        pickedUp = true;
        
        Destroy(gameObject);
    }
}
