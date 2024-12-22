using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotDisable : BaseState
{
    protected BotSM sm;
    protected Transform TF;
    public BotDisable(string name, BotSM stateMachine) : base("Disable", stateMachine)
    {
        sm = (BotSM)this.stateMachine;
    }
    public override void Enter()
    {
        base.Enter();
        //Debug.Log("asdasd");
        //sm.nav.updatePosition = false;
        //sm.nav.isStopped = true;
        sm.agent.enabled = false;
        //sm.bot.rg.useGravity = true;
        //sm.bot.rg.isKinematic = false;
    }
    public override void Exit()
    {
        base.Exit();
        //sm.nav.isStopped = false;
        //sm.nav.updatePosition = true;
        
        //sm.bot.rg.velocity = Vector3.zero;
        //sm.bot.rg.angularVelocity = Vector3.zero;
        sm.bot.rg.useGravity = false;
        sm.bot.rg.isKinematic = true;
        sm.agent.Warp(sm.transform.position);
        sm.agent.enabled = true;
    }
}
