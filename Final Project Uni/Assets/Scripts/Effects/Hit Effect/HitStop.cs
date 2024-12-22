using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    [SerializeField] private bool waiting;
    [Range(0,1)]public float slowDownTimer;
    [SerializeField] public float stopDuration;
    [SerializeField] public float delay;

    [Button]
    public void Stop()
    {
        if (waiting) return;
        
        StartCoroutine(Wait(stopDuration, delay));
    }

    public IEnumerator Wait(float duration, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = slowDownTimer;
        waiting = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        waiting = false;
    }
}
