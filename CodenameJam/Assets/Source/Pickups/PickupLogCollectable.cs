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
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        pickupLogUI.StartOpening(data);
        audioSource.Play();
        OnCollect?.Invoke();
        Destroy(gameObject);
    }
}
