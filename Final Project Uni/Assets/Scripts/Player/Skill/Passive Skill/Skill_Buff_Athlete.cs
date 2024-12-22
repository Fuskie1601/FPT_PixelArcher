using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Buff_Athlete : ISkill
{
    public float rollControl = 0.3f;
    public float bonusRollCD = 0.1f;
    public float bonusSpeed = 1f;
    public int bonusRegenRate = 2;
    
    void Start()
    {
        SetStat();
    }

    void SetStat()
    {
        if (_pc == null) _pc = PlayerController.Instance;
        _pc._stats.bonusControlRollDirect += rollControl;
        _pc._stats.bonusRollCD += bonusRollCD;
        _pc._stats.bonusSpeed += bonusSpeed;
        _pc._stats.bonusRegenRate += bonusRegenRate;
        
        _pc._stats.UpdateStats();
    }

    public override void Deactivate()
    {
        _pc._stats.bonusControlRollDirect -= rollControl;
        _pc._stats.bonusRollCD -= bonusRollCD;
        _pc._stats.bonusSpeed -= bonusSpeed;
        _pc._stats.bonusRegenRate -= bonusRegenRate;
        
        _pc._stats.UpdateStats();
    }
}
