using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Buff_Splitshot : ISkill
{
    void Start()
    {
        if(_pc == null) _pc = PlayerController.Instance;
        SetToggle(true);
    }
    void SetToggle(bool toggle)
    {
        //Name = "Split shot";
        //type = SkillType.PASSIVE;
        _pc._arrowController.IsSplitShot = toggle;
    }

    public override void Deactivate()
    {
        SetToggle(false);
    }
}
