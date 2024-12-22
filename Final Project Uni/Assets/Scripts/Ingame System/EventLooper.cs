using UnityEngine;
using UnityEngine.Events;

public class EventLooper : MonoBehaviour
{
    [Header("Settings")]
    public bool Activate = false; // Controls whether the event loop is active
    public float timeLoop = 1f;   // Interval in seconds between each event call

    [Header("Unity Event")]
    public UnityEvent OnLoopEvent; // The UnityEvent to invoke at each interval

    private float timer = 0f;

    private void Update()
    {
        // Check if the looper is active
        if (Activate)
        {
            // Increment timer
            timer += Time.deltaTime;

            // Check if it's time to trigger the event
            if (timer >= timeLoop)
            {
                timer = 0f; // Reset timer
                OnLoopEvent?.Invoke(); // Invoke the event
            }
        }
    }

    /// <summary>
    /// Starts or stops the event looper.
    /// </summary>
    /// <param name="state">Set to true to activate, false to deactivate.</param>
    public void SetActivate(bool state)
    {
        Activate = state;
        if (!Activate)
            timer = 0f; // Reset timer when deactivated
    }
}