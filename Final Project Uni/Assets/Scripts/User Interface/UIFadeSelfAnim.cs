using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class UIFadeSelfAnim : MonoBehaviour
{
    public Animator _animator;
    public float FadeInDuration = 0.5f, FadeOutDuration = 0.5f;
    public UnityEvent FadeInFin, FadeInFin2, FadeOutFin, FadeOutFin2;

    public void doFadeIn(bool secondFin = false)
    {
        //Debug.Log("FadeIn");
        _animator.SetTrigger("FadeIn");
        FadeInFinish(FadeInDuration, secondFin);
    }
    public void doFadeOut(bool secondFin = false)
    {
        //Debug.Log("FadeOut");
        _animator.SetTrigger("FadeOut");
        FadeOutFinish(FadeOutDuration, secondFin);
    }

    async void FadeInFinish(float delay, bool secondFin)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        
        if(!secondFin)
            FadeInFin.Invoke();
        else
            FadeInFin2.Invoke();
    }
    async void FadeOutFinish(float delay, bool secondFin)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        
        if(!secondFin)
            FadeOutFin.Invoke();
        else
            FadeOutFin2.Invoke();
    }
    

    public void ToggleObjectOn()
    {
        gameObject.SetActive(true);
    }
    
    public void ToggleObjectOff()
    {
        gameObject.SetActive(false);
    }
    
    public void TogglePlayerUI(bool toggle)
    {
        ToggleUIElements.Instance.ToggleUISkip(toggle, 0);
    }
    
    //Temp
    public void ToggleRevPlayerUI(bool toggle)
    {
        Debug.Log("nyaaaaa");
        ToggleUIElements.Instance.ToggleUI(toggle);
    }
}
