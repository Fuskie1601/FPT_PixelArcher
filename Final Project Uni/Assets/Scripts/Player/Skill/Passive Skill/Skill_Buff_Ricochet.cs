using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Buff_Ricochet : ISkill
{
    public float ricochetValue = 1f;
    
    void Start()
    {
        SetStat();
    }
    
    void SetStat()
    {
        if(_pc == null) _pc = PlayerController.Instance;
        //Debug.Log("Add Ricochet!!");
        _pc._stats.bonusRicochetMultiplier = ricochetValue;
        _pc._stats.UpdateStats();
    }

    public override void Deactivate()
    {
        if(_pc == null) _pc = PlayerController.Instance;
        _pc._stats.bonusRicochetMultiplier = 0;
        _pc._stats.UpdateStats();
    }
}
