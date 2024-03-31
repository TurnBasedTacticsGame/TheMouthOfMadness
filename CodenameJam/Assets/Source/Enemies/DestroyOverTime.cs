using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOverTime : MonoBehaviour
{
    private bool started;
    private float timer = 0;

    private void Update()
    {
        if (!started)
            return;

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void StartTimer(float time)
    {
        timer = time;
    }
}
