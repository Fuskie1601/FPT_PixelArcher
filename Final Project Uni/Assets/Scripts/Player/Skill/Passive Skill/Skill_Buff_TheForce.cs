using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Buff_TheForce : ISkill
{
    public float bonusRecallSpeed = 10f;
    
    void Start()
    {
        SetStat();
    }
    
    void SetStat()
    {
        //Debug.Log("Add Ricochet!!");
        if(_pc == null) _pc = PlayerController.Instance;
        _pc._stats.bonusRecallSpeed += bonusRecallSpeed;
        _pc._stats.UpdateStats();
    }

    public override void Deactivate()
    {
        if(_pc == null) _pc = PlayerController.Instance;
        _pc._stats.bonusRecallSpeed -= bonusRecallSpeed;
        _pc._stats.UpdateStats();
    }
}
