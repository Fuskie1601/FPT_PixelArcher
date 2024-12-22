using UnityEngine;
using TMPro;

public class SpeedrunTimer : MonoBehaviour
{
    // Reference to the TextMeshPro component
    public TextMeshProUGUI timerText;

    // Variable to store the time elapsed
    private float elapsedTime = 0f;
    private bool isRunning = true;

    void Update()
    {
        // If the timer is running, update the elapsed time
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;

            // Calculate minutes, seconds, and milliseconds
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            int milliseconds = Mathf.FloorToInt((elapsedTime * 1000f) % 1000);

            // Update the TextMeshPro text in the format "MM:SS:MS"
            timerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        }
    }

    // Method to start the timer
    public void StartTimer()
    {
        isRunning = true;
    }

    // Method to stop the timer
    public void StopTimer()
    {
        isRunning = false;
    }

    // Method to reset the timer
    public void ResetTimer()
    {
        elapsedTime = 0f;
        timerText.text = "00:00:000";
    }
}
