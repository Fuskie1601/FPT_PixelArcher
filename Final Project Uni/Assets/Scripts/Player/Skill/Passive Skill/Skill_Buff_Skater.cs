using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Buff_Skater : ISkill
{
    public float drag = 2f;
    public float bonusSpeed = 4f;
    
    void Start()
    {
        SetStat();
    }

    void SetStat()
    {
        if(_pc == null) _pc = PlayerController.Instance;
        _pc.PlayerRB.drag = drag;
        _pc._stats.bonusSpeed += bonusSpeed;
    }
    
    public override void Deactivate()
    {
        if(_pc == null) _pc = PlayerController.Instance;
        _pc.PlayerRB.drag = _pc._stats.defaultDrag;
        _pc._stats.bonusSpeed -= bonusSpeed;
        _pc._stats.UpdateStats();
    }
}
