using UniDi;
using UnityEngine;

public class PickupLogCollectable : MonoBehaviour
{
    [SerializeField] private TextData data;

    [Inject] private PickupLogUI pickupLogUI;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        pickupLogUI.StartOpening(data);
        Destroy(gameObject);
    }
}
