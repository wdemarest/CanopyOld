using UnityEngine;
using System.Collections;
using System;

public class Timer : MonoBehaviour
{
    public float duration = 0.0f;
    public float remaining = 0.0f;
    public Action callback;

    public void Set(float _duration, Action _callback)
    {
        duration = _duration;
        remaining = duration;
        callback = _callback;
    }

    public void Reset()
    {
        remaining = duration;
    }

    public void Update()
    {
        remaining -= Time.deltaTime;
        if (remaining <= 0.0f)
        {
            callback();
        }

    }
}