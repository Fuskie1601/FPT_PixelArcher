using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
 
public class SceneLoader : MonoBehaviour
{
    public GameObject LoaderUI;
    public Slider progressSlider;
    [CanBeNull] public UIFadeSelfAnim FadeSelfAnim;
    public UnityEvent CompletedLoad, BarFulled;
    public bool doRemoveInstance;

    private void Awake()
    {
        if (FadeSelfAnim == null) FadeSelfAnim = GetComponent<UIFadeSelfAnim>();
    }

    private void Start()
    {
        if(Manager_Singleton.Instance != null && doRemoveInstance) Destroy(Manager_Singleton.Instance.gameObject);
    }

    private void OnEnable()
    {
        if (FadeSelfAnim != null) FadeSelfAnim.doFadeIn();
    }

    public void LoadScene(int index)
    {
        LoaderUI.SetActive(true);
        StartCoroutine(LoadScene_Coroutine(index));
    }
 
    public IEnumerator LoadScene_Coroutine(int index)
    {
        progressSlider.value = 0;
        LoaderUI.SetActive(true);
 
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index);
        asyncOperation.allowSceneActivation = false;
        float progress = 0;
 
        while (!asyncOperation.isDone)
        {
            progress = Mathf.MoveTowards(progress, asyncOperation.progress, Time.deltaTime);
            progressSlider.value = progress;
            if (progress >= 0.9f)
            {
                progressSlider.value = 1;
                yield return new WaitForSeconds(0.25f);
                BarFulled?.Invoke();
                yield return new WaitForSeconds(0.5f);
                asyncOperation.allowSceneActivation = true;
                CompletedLoad?.Invoke(); 
            }
            yield return null;
        }
    }
    
    public void PlayBGMBiome()
    {
        doPlayBGMBiome();
    }
    
    public async void doPlayBGMBiome()
    {
        // Wait until ExpeditionManager.Instance is assigned
        await UniTask.WaitUntil(() => ExpeditionManager.Instance.currentBiome != null);

        // Call PlayBGMBiome after ExpeditionManager is ready
        ExpeditionManager.Instance.PlayBGMBiome();
    }
}