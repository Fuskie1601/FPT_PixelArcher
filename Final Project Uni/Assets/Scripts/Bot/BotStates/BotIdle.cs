using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotIdle : BaseState
{
    protected BotSM sm;
    public BotIdle(BotSM stateMachine) : base("Idle", stateMachine)
    {
        sm = (BotSM)this.stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        sm.currState = "Idle";
        
        if(sm.bot.unitType != UnitType.BossPattern)
        sm.ChangeState(sm.StrafeState);

    }
}
