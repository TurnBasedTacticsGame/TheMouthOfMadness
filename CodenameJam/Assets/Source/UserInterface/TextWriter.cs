using System;
using System.Collections;
using System.Collections.Generic;
using UniDi;
using UnityEngine;

// This is a very roundabout implementation, but it works!
public class TextWriter : MonoBehaviour
{
    [SerializeField] private RandomAudioPlayer randomAudioPlayer;
    
    public event Action<string> OnWriteCharacter;
    
    private TextData textData;
    private bool writing;
    private int currentTextGroupIndex;
    private int currentCharacterIndex;
    private float nextCharacterTimer;
    private int charactersWritten;
    
    private void Update()
    {
        if (!writing)
        {
            return;
        }
        
        nextCharacterTimer -= Time.unscaledDeltaTime;

        if (nextCharacterTimer <= 0)
        {
            if (TryGetNextString(out var character))
            {
                charactersWritten++;
                OnWriteCharacter?.Invoke(character);
                // Hacky
                if (character.Length == 1 && charactersWritten % 3 == 0)
                {
                    randomAudioPlayer.PlayRandomOnce(textData.TextGroups[currentTextGroupIndex].audioClipData);
                }
                nextCharacterTimer = 0.01f;
            }
            else
            {
                writing = false;
            }
        }
    }
    
    public void StartWriting(TextData newLogData)
    {
        if (writing) return;
        
        textData = newLogData;
        currentTextGroupIndex = 0;
        currentCharacterIndex = 0;
        writing = true;
    }

    public void StopWriting()
    {
        writing = false;
    }

    private bool TryGetNextString(out string character)
    {
        if (currentTextGroupIndex >= textData.TextGroups.Length)
        {
            character = "";
            return false;
        }

        if (currentCharacterIndex >= textData.TextGroups[currentTextGroupIndex].text.Length)
        {
            currentCharacterIndex = 0;
            currentTextGroupIndex++;
            
            // Check again
            if (currentTextGroupIndex >= textData.TextGroups.Length 
                || currentCharacterIndex >= textData.TextGroups[currentTextGroupIndex].text.Length)
            {
                character = "";
                return false;
            }
        }

        var text = textData.TextGroups[currentTextGroupIndex].text;

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
