using System;
using System.Collections;
using System.Collections.Generic;
using Source.GameEvents.Core;
using Source.Players;
using UniDi;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Inject] private GameContext gameContext;
    [Inject] private Player player;

    public SpawnPoint debugActive;

    private void OnEnable()
    {
        player.OnPlayerDeath += RespawnPlayer;
    }

    private void OnDisable()
    {
        player.OnPlayerDeath -= RespawnPlayer;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.attachedRigidbody != null && 
            col.attachedRigidbody.TryGetComponent<Player>(out var _))
        {
            Debug.Log("Collided with player");
            gameContext.ActiveSpawnPoint = this;
            gameObject.SetActive(false);
        }
    }

    private void RespawnPlayer()
    {
        if (!gameContext.ActiveSpawnPoint)
        {
            return;
        }
        
        player.transform.position = gameContext.ActiveSpawnPoint.transform.position;
        player.Respawn();
    }
}
