using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class AudioManager : SerializedMonoBehaviour
{
    #region Variables
    public static AudioManager Instance { get; private set; }
    [SerializeField] private AudioSource audioSource, musicSource, musicSource2;

    [SerializeField] private Dictionary<string, AudioClip> soundEffectsDict;
    [SerializeField] private Dictionary<string, AudioClip> backgroundMusicDict;
    [SerializeField] private AnimationCurve transitionCurve;
    [Range(0, 1)] public float volume;

    public string playingBGM;
    #endregion

    #region Unity Method
    public void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
    }
    #endregion

    #region Methods

    public void PlaySoundEffect(string soundName)
    {
        if (soundEffectsDict.TryGetValue(soundName, out var clip))
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Sound effect '{soundName}' not found in the dictionary!");
        }
    }

    public void PlayBackgroundMusic(string sceneNumber)
    {
        if (backgroundMusicDict.TryGetValue(sceneNumber, out var clip))
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Background music for scene '{sceneNumber}' not found in the dictionary!");
        }
    }

    private async UniTask TransitionMusicAsync(string backgroundMusic)
    {
        playingBGM = backgroundMusic;
        float time = 0f;
        float duration = 1f;

        if (!backgroundMusicDict.TryGetValue(backgroundMusic, out var newClip))
        {
            Debug.Log($"Background music for scene '{backgroundMusic}' not found in the dictionary!");
            return;
        }

        musicSource2.clip = newClip;
        musicSource2.volume = 0f;
        musicSource2.Play();

        while (time < duration)
        {
            time += Time.unscaledDeltaTime; // Use unscaledDeltaTime to handle scene loading disruptions
            float volumeTransition = transitionCurve.Evaluate(time / duration); // Normalize time for transition
            musicSource.volume = (1 - volumeTransition) * volume;
            musicSource2.volume = volumeTransition * volume;
            await UniTask.Yield(); // Yield back to the main thread to avoid blocking
        }

        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        // Complete the transition
        musicSource.Stop();
        musicSource.volume = volume; // Reset to default volume
        (musicSource, musicSource2) = (musicSource2, musicSource); // Swap references
    }


    private async UniTask<AudioClip> LoadAudioClipAsync(string sceneNumber)
    {
        if (backgroundMusicDict.TryGetValue(sceneNumber, out var clip))
        {
            // Simulate asynchronous loading
            return clip;
        }
        return null;
    }
    #endregion

    #region Ults
    [FoldoutGroup("Event Test")]
    [Button]
    public void PlaySound(string soundName)
    {
        AudioManager.Instance.PlaySoundEffect(soundName);
    }

    [FoldoutGroup("Event Test")]
    [Button]
    public void PlaySceneMusic(string sceneNumber)
    {
        PlayBackgroundMusic(sceneNumber);
    }

    [FoldoutGroup("Event Test")]
    [Button]
    public void ChangeMusic(string BGMId)
    {
        TransitionMusicAsync(BGMId).Forget();
    }

    #endregion
}