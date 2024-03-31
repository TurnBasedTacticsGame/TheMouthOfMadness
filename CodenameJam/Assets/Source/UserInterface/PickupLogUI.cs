using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniDi;
using UnityEngine;

public class PickupLogUI : MonoBehaviour
{
    public bool PlayerIsReading { get; set; }
    
    public event Action OnPickupTextClose; 

    [Header("Dependencies")]
    [SerializeField] private TextMeshProUGUI textUi;
    [SerializeField] private Animator animator;
    [SerializeField] private TextWriter textWriter;

    private TextData currentLogData;
    private static readonly int Opened = Animator.StringToHash("Opened");

    private void OnEnable()
    {
        textWriter.OnWriteCharacter += WriteToUi;
        textUi.text = "";
    }
    
    private void OnDisable()
    {
        textWriter.OnWriteCharacter -= WriteToUi;
    }
    
    public void StartOpening(TextData newLogData)
    {
        gameObject.SetActive(true);
        animator.SetBool(Opened, true);
        currentLogData = newLogData;
        PlayerIsReading = true;
    }
    
    // Used by button
    public void StartClosing()
    {
        animator.SetBool(Opened, false);
    }
    
    // Used by animator events
    public void Open()
    {
        gameObject.SetActive(true);
        textUi.gameObject.SetActive(true);
        textWriter.StartWriting(currentLogData);
    }

    // Used by animator events
    public void Close()
    {
        textWriter.StopWriting();
        OnPickupTextClose?.Invoke();
        gameObject.SetActive(false);
        textUi.gameObject.SetActive(false);
        PlayerIsReading = false;
    }

    private void WriteToUi(string characters)
    {
        textUi.text += characters;
    }
}
