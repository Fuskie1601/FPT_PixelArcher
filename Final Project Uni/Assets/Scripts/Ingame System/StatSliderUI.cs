using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class StatSliderUI : MonoBehaviour
{
    private Slider hpSlider;
    [ReadOnly] public float currentValue, targetValue;
    public float currentVelocity;
    [SerializeField, ReadOnly] private bool isUpdating;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float showHideSpeed = 0.1f;
    public bool toggleShow = true;
    
    public bool autoFill = true;
    [SerializeField] private Image Background, Fill;

    private void Awake()
    {
        canvasGroup = GetComponentInParent<CanvasGroup>();
        hpSlider = GetComponent<Slider>();
        if (autoFill) targetValue = 1;
        else targetValue = 0;
        isUpdating = true;
    }

    private void Start()
    {
        UpdateToggle(toggleShow);
    }

    private void Update()
    {
        if (isUpdating)
        {
            currentValue = Mathf.SmoothDamp(currentValue, targetValue, ref currentVelocity, 0.1f);
            hpSlider.value = currentValue;
            if (Mathf.Approximately(currentValue, targetValue)) isUpdating = false;
        }
    }
    
    [Button]
    public void UpdateValue(float updatedHP, float maxHP)
    {
        float value = updatedHP / maxHP;
        targetValue = value;
        isUpdating = true;
    }
    
    public async UniTaskVoid UpdateToggle(bool toggle, float delay = 0)
    {
        if (canvasGroup == null) return;

        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        toggleShow = toggle;

        // Show the bar by setting alpha to 1
        if (toggleShow)
            canvasGroup.DOFade(1, showHideSpeed);
        else
            canvasGroup.DOFade(0, showHideSpeed);
    }
}
