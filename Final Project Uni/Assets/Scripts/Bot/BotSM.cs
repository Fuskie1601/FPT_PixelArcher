using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

public class BotSM : StateMachine
{
    //public List<Transform> targets;
    public Rigidbody target;
    public Vector3 destination;
    public NavMeshAgent agent;
    public Transform defaultDestination;
    public BotMain bot;
    public string currState;
    public bool isAlive = true;
    public float distance;

    [CanBeNull] public BossSkill BossSkill;
    
    [HideInInspector] public BotIdle idleState;
    [HideInInspector] public BotChase chaseState;
    [HideInInspector] public BotAttacking AttackingState;
    [HideInInspector] public BotStrafe StrafeState;
    [HideInInspector] public BotKnockback knockbackState;
    [HideInInspector] public BotDeath deathState;
    
    [HideInInspector] public BotPattern BotPattern;

    public void Awake()
    {
        idleState = new BotIdle(this);
        AttackingState = new BotAttacking(this);
        chaseState = new BotChase(this);
        StrafeState = new BotStrafe(this);
        knockbackState = new BotKnockback(this);
        deathState = new BotDeath(this);
        BotPattern = new BotPattern(this);

        if (bot.gun != null && BossSkill != null)
        {
            BossSkill.guns = bot.gun;
            BossSkill.sm = this;
        }
        //targets = new List<Transform>();
    }
    
    public void GoIdle()
    {
        ChangeState(idleState);
    }
    protected override BaseState GetInitialState()
    {
        return idleState;
    }
    public void GoDeath()
    {
        isAlive = false;
        ChangeState(deathState);
    }
    public void GoKnockback(Vector3 force)
    {
        if (currState != "Knockback")
        {
            knockbackState.force = force;
            ChangeState(knockbackState);
        }
    }

    public void Teleport(Vector3 pos)
    {
        agent.Warp(pos);
    }
}
