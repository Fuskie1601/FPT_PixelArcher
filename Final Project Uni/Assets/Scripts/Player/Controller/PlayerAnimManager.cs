using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimManager : MonoBehaviour
{
    public Animator playerAnimator;
    [CanBeNull] public Animator bowPivotAnimator;
    [CanBeNull] public Animator bowAnimator;

    public bool isDrawed;

    private void Awake()
    {
        if(playerAnimator == null) playerAnimator = GetComponentInChildren<Animator>();
    }

    #region Player

    public void UpdateRunInput(bool moving)
    {
        playerAnimator.SetBool("Moving", moving);
    }
    public void GuardAnim(bool guard)
    {
        playerAnimator.SetBool("Guard", guard);
    }
    public void DodgeAnim()
    {
        playerAnimator.SetTrigger("Dodge");
    }
    public void DieAnim(bool die)
    {
        playerAnimator.SetBool("Die", die);
        if(die) playerAnimator.SetTrigger("DieTrigger");

        if(die) ResetBow();
        //UpdateHaveArrow(die);
    }
    
    [Button]
    public void DamagedAnim()
    {
        playerAnimator.SetTrigger("Damaged");
    }
    public void RecallAnim(bool Recalling, bool ReverseRecalling = false)
    {
        playerAnimator.SetBool("Recalling", Recalling);
        playerAnimator.SetBool("ReverseRecalling", ReverseRecalling);
    }
    public void RecallAnimTrigger()
    {
        playerAnimator.SetTrigger("RecallingStart");
    }

    public async UniTaskVoid DebuffStun(float time)
    {
        playerAnimator.SetBool("Stun", true);

        await UniTask.Delay(TimeSpan.FromSeconds(time));
        
        playerAnimator.SetBool("Stun", false);
    }

    #endregion

    #region Bow

    public void UpdateHaveArrow(bool haveArrow)
    {
        bowAnimator.SetBool("haveArrow", haveArrow);
        bowPivotAnimator.SetBool("haveArrow", haveArrow);
    }
    
    public void ResetBow()
    {
        bowAnimator.Rebind();
        bowPivotAnimator.Rebind();
    }
    
    [Button]
    public void Draw(bool bowShoot, bool changeDraw)
    {
        if(bowPivotAnimator == null || bowAnimator == null) return;
        
        if(changeDraw) isDrawed = !isDrawed;
        bowPivotAnimator.SetBool("Charging", isDrawed);
        bowAnimator.SetBool("Charging", isDrawed);
        bowPivotAnimator.SetTrigger("ShootTrigger");
        
        if(bowShoot) ShootBow();
    }

    public async UniTaskVoid ShootBow()
    {
        bowAnimator.SetTrigger("ShootTrigger");
    }

    public async UniTaskVoid Slash()
    {
        bowPivotAnimator.SetTrigger("Slash");
        await UniTask.Delay(TimeSpan.FromSeconds(.01f));
        bowPivotAnimator.ResetTrigger("Slash");

    }

    #endregion
}
