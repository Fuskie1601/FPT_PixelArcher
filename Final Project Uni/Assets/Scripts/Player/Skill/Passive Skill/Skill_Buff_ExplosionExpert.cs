using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Buff_ExplosionExpert : ISkill
{
    void Start()
    {
        SetToggle(true);
    }
    
    void SetToggle(bool toggle)
    {
        if(_pc == null) _pc = PlayerController.Instance;
        
        foreach (var arrow in _pc._arrowController.arrowsList)
        {
            if(arrow.IsMainArrow) arrow.isExplodeOnRemote = true;
        }
    }
    
    public override void Deactivate()
    {
        SetToggle(false);
    }
}
