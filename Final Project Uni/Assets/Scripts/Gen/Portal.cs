using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [HideInInspector] public ExpeditionManager exManager;
    public bool interacted = true;
    public void Start()
    {
        exManager = ExpeditionManager.Instance;
    }

    private void OnEnable()
    {
        interacted = true;
    }

    [Button]
    public void StartPortal()
    {
        if(!interacted) return;

        interacted = false;
        exManager.ExitFloor();
    }

    public void ActivatePortal()
    {
        gameObject.SetActive(true);
    }

    public void HubPortal()
    {
        //GameManager.Instance.changeColorTransition.Invoke(Color.gray);
        GameManager.Instance.StartExpedition();
        PlayerController.Instance._UIContainer.FadeIn();
    }
}
