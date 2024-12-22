using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public enum UIState
{
    Gameplay, Event, Inventory
}

public class UIContainer : MonoBehaviour
{
    public UIState CurrentUIState;

    [FoldoutGroup("UI Setup")] public List<GameObject> Gameplay;
    [FoldoutGroup("UI Setup")] public GameObject SkillChoose, Inventory;
    [FoldoutGroup("UI Setup")] public Image FadeImage;

    public SelectRandomSkillEvent selectRandomSkillEvent;

    //public Animator FadeAnimator;
    public UIFadeSelfAnim FadeAnimator;

    void OnEnable()
    {
        selectRandomSkillEvent = SkillChoose.GetComponent<SelectRandomSkillEvent>();

        StartFadeOut(0.5f);
    }

    private void Start()
    {
        FadeAnimator = Manager_Singleton.Instance.GetComponentInChildren<UIFadeSelfAnim>();
    }

    public async void StartFadeOut(float delay = 0)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        FadeAnimator.doFadeOut();
    }

    [ButtonGroup("State")]
    public void GameplayState()
    {
        CurrentUIState = UIState.Gameplay;

        SkillChoose.SetActive(false);
        Inventory.SetActive(false);

        foreach (var obj in Gameplay)
        {
            obj.SetActive(true);
        }
    }

    [ButtonGroup("State")]
    public void InventoryState()
    {
        CurrentUIState = UIState.Inventory;

        foreach (var obj in Gameplay)
        {
            obj.SetActive(false);
        }

        Inventory.SetActive(true);
    }

    [ButtonGroup("State")]
    public void SkillChooseState()
    {
        CurrentUIState = UIState.Event;

        SkillChoose.SetActive(true);
        foreach (var obj in Gameplay)
        {
            obj.SetActive(false);
        }

        selectRandomSkillEvent.SkillSelectStart();
    }

    public void TransitionState()
    {
        foreach (var obj in Gameplay)
        {
            obj.SetActive(false);
        }
    }


    public void SetColorFade(Color color = default)
    {
        if (color == default) color = Color.black; // Set default to black if not provided

        Color currentColor = FadeImage.color;
        FadeImage.color = new Color(color.r, color.g, color.b, currentColor.a);
    }

    public void FadeAnim()
    {
        foreach (var obj in Gameplay)
        {
            obj.gameObject.SetActive(false);
        }
    }

    IEnumerator FadeInAnimation()
    {
        Debug.Log("Fade In");
        FadeAnim();
        FadeAnimator.doFadeIn();

        yield return new WaitForSeconds(1.5f);
    }

    IEnumerator FadeOutAnimation()
    {
        Debug.Log("Fade Out");
        FadeAnim();
        StartFadeOut(0.5f);
        //FadeAnimator.doFadeOut();
        yield return new WaitForSeconds(2f);

        if (CurrentUIState != UIState.Event)
            GameplayState();
    }

    [Button]
    public void FadeIn()
    {
        StartCoroutine(FadeInAnimation());
    }


    [Button]
    public void FadeOut()
    {
        StartCoroutine(FadeOutAnimation());
    }

    //Temp
    public void LoadLobby()
    {
        GameManager.Instance.StartLobby();
    }
}