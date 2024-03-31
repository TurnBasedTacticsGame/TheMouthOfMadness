using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PickupLogUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textUi;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Open(PickupLogData pickupLogData)
    {
        gameObject.SetActive(true);
        textUi.gameObject.SetActive(true);
        textUi.text = pickupLogData.Text;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        textUi.gameObject.SetActive(false);
    }
}
