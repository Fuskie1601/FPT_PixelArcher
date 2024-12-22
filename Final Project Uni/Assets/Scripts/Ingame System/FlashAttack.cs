using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class FlashAttack : MonoBehaviour
{
    [CanBeNull] public HurtBox hb;
    public bool playOnStart = true, doCamShake;
    public float delay = 1;
    public float HitDuration = 0.5f;
    public float CamShakeAmount = 0.1f;
    public string soundEffect;

    [CanBeNull] public GameObject Indicator;

    public UnityEvent Strike, EndStrike;

    private void Start()
    {
        if(playOnStart) doFlash();
    }

    [Button]
    public void doFlash()
    {
        doFlashAttack(HitDuration, delay);
    }

    private void OnEnable()
    {
        if(Indicator != null) 
            Indicator.SetActive(true);
        doFlash();
    }

    async UniTaskVoid doFlashAttack(float hitDuration, float delay = 0)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));

        if(hb != null) hb.Activate = true;
        Strike.Invoke();
        
        if(doCamShake) CamShake.Instance.AddTrauma(CamShakeAmount, true);
        
        await UniTask.Delay(TimeSpan.FromSeconds(hitDuration));
        
        if(hb != null) hb.Activate = false;
        EndStrike.Invoke();
    }

    public void IndicatorHide()
    {
        if(Indicator != null) 
            Indicator.SetActive(false);
    }

    public void playSoundEffect(string soundEffect) 
    {
        AudioManager.Instance.PlaySoundEffect(soundEffect);
    }

}
