using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Singleton : MonoBehaviour
{
    public static Player_Singleton Instance;

    private void Start()
    {
        if (Instance == null)
        {
            Debug.Log("yay");
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("kaboom");
            Destroy(gameObject); // Destroy duplicate
        }
    }
    
    private void OnEnable()
    {
        // Subscribe to the scene-loaded event
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe from the scene-loaded event
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        EnsureSingleAudioListener();
    }

    private void EnsureSingleAudioListener()
    {
        // Get all active AudioListeners in the scene
        AudioListener[] audioListeners = FindObjectsOfType<AudioListener>();

        // Check if there's already an active AudioListener
        bool isAudioListenerActive = false;

        foreach (AudioListener listener in audioListeners)
        {
            if (listener.enabled)
            {
                if (!isAudioListenerActive)
                {
                    // Keep the first active AudioListener
                    isAudioListenerActive = true;
                }
                else
                {
                    // Disable any additional active AudioListeners
                    listener.enabled = false;
                }
            }
        }

        // If no AudioListener is active, enable the one on this GameObject
        if (!isAudioListenerActive)
        {
            AudioListener localListener = GetComponent<AudioListener>();
            if (localListener != null)
            {
                localListener.enabled = true;
            }
        }
    }
}
