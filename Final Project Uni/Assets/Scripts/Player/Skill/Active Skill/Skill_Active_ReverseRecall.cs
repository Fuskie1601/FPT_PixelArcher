using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Active_ReverseRecall : ISkill
{
    public float ReverseRecallMultiplier = 1;
    public float RRMass = 1, RRDrag = 2;
    
    private void Start()
    {
        if(_pc == null) _pc = PlayerController.Instance;
        _pc._playerMovementManager.ReverseRecallMultiplier = ReverseRecallMultiplier;
    }
    
    public override void Activate()
    {
        if (currentCD <= 0) ReverseRecall(true);
    }

    public override void Deactivate()
    {
        ReverseRecall(false);
    }

    public void ReverseRecall(bool toggle)
    {
        if (toggle && !_pc._arrowController.haveArrow)
        {
            Debug.Log(Name + " Activated");
            currentCD = Cooldown;
            _pc.staminaSystem.canRegen = false;
            _pc.currentState = PlayerState.ReverseRecalling;
            _pc.PlayerRB.drag = RRDrag;
            _pc.PlayerRB.mass = RRMass;
        }
        else
        {
            _pc.staminaSystem.canRegen = true;
            _pc.currentState = PlayerState.Idle;
            _pc.PlayerRB.drag = _pc._stats.defaultDrag;
            _pc.PlayerRB.mass = _pc._stats.defaultMass;
            currentCD = Cooldown;
        }
    }
}
