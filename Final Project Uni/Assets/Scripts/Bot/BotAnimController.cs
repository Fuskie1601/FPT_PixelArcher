using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class BotAnimController : MonoBehaviour
{
    public Animator botAnimator;
    //public Rig rig;
    
    private void Awake()
    {
        if(botAnimator == null) botAnimator = GetComponentInChildren<Animator>();
        //if (rig == null) rig = GetComponentInChildren<Rig>();
    }

    private void OnDisable()
    {
        //rig.weight = 1;
    }

    public void UpdateRunInput(bool moving)
    {
        botAnimator.SetBool("Moving", moving);
    }
    
    public void GuardAnim(bool guard)
    {
        botAnimator.SetBool("Guard", guard);
    }
    
    [Button]
    public async UniTaskVoid DamagedAnim()
    {
        botAnimator.Play("Damaged");
    }
    
    [Button("Attack")]
    public void AttackAnim(bool Attacking)
    {
        if(Attacking) botAnimator.SetTrigger("AttackStart");
        botAnimator.SetBool("Attacking", Attacking);
    }
    
    [Button("Special Attack")]
    public void SpecialAttackAnim(bool Attacking, int SpecialType = 0)
    {
        botAnimator.SetInteger("AttackType", SpecialType);
        
        if(Attacking) botAnimator.SetTrigger("SpecialAttackStart");
        botAnimator.SetBool("Attacking", Attacking);
    }
    
    public async UniTaskVoid DebuffStun(float time)
    {
        botAnimator.SetBool("Stun", true);
        botAnimator.SetTrigger("StunStart");
        await UniTask.Delay(TimeSpan.FromSeconds(time));
        botAnimator.SetBool("Stun", false);
    }

    public void Die()
    {
        //if (rig != null) rig.weight = 0;
        botAnimator.SetTrigger("Die");
    }
    
    
}
