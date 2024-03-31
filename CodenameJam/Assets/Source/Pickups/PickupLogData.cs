using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pickup Log Data", menuName = "Pickup Log Data")]
public class PickupLogData : ScriptableObject
{
    public TextGroup[] TextGroups => textGroups;
    
    [SerializeField] private TextGroup[] textGroups;

    [Serializable]
    public struct TextGroup
    {
        [TextArea(5, 50)]
        public string text;
        [Range(0.1f, 1f)]
        public float timePerCharacter;
    }
}
