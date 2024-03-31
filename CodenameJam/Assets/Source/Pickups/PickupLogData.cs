using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pickup Log Data", menuName = "Pickup Log Data")]
public class PickupLogData : ScriptableObject
{
    public string Text => text;
    
    [TextArea(5, 50)]
    [SerializeField] private string text;
}
