using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotDeath : BotDisable
{
    protected float cooldown = 0.1f;
    protected float countdown;
    public BotDeath(BotSM stateMachine) : base("Knockback", stateMachine)
    {
        sm = (BotSM)this.stateMachine;
    }
    public override void Enter()
    {
        base.Enter();
        sm.currState = "Death";
        
        if(sm.bot._animController != null)
            sm.bot._animController.Die();

        sm.bot.enabled = false;
    }

    // Update is called once per frame
    public override void UpdateLogic()
    {
        base.UpdateLogic();
    }
}
