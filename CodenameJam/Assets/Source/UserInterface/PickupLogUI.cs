using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PickupLogUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textUi;

    private PickupLogData pickupLogData;
    private bool finished;
    private int currentTextGroupIndex;
    private int currentCharacterIndex;
    private float nextCharacterTimer;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (finished)
        {
            return;
        }
        
        nextCharacterTimer -= Time.unscaledDeltaTime;

        if (nextCharacterTimer <= 0)
        {
            if (TryGetNextString(out var character))
            { 
                textUi.text += character;
                nextCharacterTimer = pickupLogData.TextGroups[currentTextGroupIndex].timePerCharacter;
            }
            else
            {
                finished = true;
            }
        }
    }

    public void Open(PickupLogData newLogData)
    {
        gameObject.SetActive(true);
        textUi.gameObject.SetActive(true);
        textUi.text = "";
        
        pickupLogData = newLogData;
        currentTextGroupIndex = 0;
        currentCharacterIndex = 0;
        finished = false;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        textUi.gameObject.SetActive(false);
        finished = true;
    }

    private bool TryGetNextString(out string character)
    {
        if (currentTextGroupIndex >= pickupLogData.TextGroups.Length)
        {
            character = "";
            return false;
        }

        if (currentCharacterIndex >= pickupLogData.TextGroups[currentTextGroupIndex].text.Length)
        {
            currentCharacterIndex = 0;
            currentTextGroupIndex++;
            
            // Check again
            if (currentTextGroupIndex >= pickupLogData.TextGroups.Length 
                || currentCharacterIndex >= pickupLogData.TextGroups[currentTextGroupIndex].text.Length)
            {
                character = "";
                return false;
            }
        }

        var text = pickupLogData.TextGroups[currentTextGroupIndex].text;

        var potentialCharacter = text[currentCharacterIndex];
        character = potentialCharacter.ToString();

        var characterChanged = false;
        switch (potentialCharacter)
        {
            // Tag support
            case '<':
                for (var i = currentCharacterIndex; i < text.Length; i++)
                {
                    var nextChar = text[i];
                    if (nextChar == '>')
                    {
                        var tagLength = i - currentCharacterIndex;
                        character = text.Substring(currentCharacterIndex, tagLength + 1);
                        currentCharacterIndex += tagLength + 1;
                        characterChanged  = true;
                        break;
                    }
                }

                break;
        }

        if (!characterChanged)
        {
            currentCharacterIndex++;
        }
        
        return true;
    }
}
