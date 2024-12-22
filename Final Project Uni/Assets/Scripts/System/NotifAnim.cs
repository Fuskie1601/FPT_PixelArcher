using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NotifAnim : MonoBehaviour
{
    public TMP_Text notifText;
    public Animator NotifTextAnimator;

    public void FloorNotif()
    {
        //Debug.Log("yay");
        EditText((ExpeditionManager.Instance.currentWorldNumber + 1) + " - " + 
                 (ExpeditionManager.Instance.currentFloorNumber + 1));
        Show(1);
    }
    
    public void Notif(string input)
    {
        EditText(input);
        Show();
    }
    
    void EditText(string input)
    {
        notifText.text = input;
    }
    async UniTaskVoid Show(float timeDelay = 0)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(timeDelay));
        if(NotifTextAnimator != null)
            NotifTextAnimator.SetTrigger("Show");
    }
}
