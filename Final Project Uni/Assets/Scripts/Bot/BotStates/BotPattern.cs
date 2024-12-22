using Mono.CSharp;
using UnityEngine;

public class BotPattern : BotTargeting
{

    public BotPattern(BotSM stateMachine) : base("Pattern", stateMachine)
    {
    }
    
    public override void Enter()
    {
        base.Enter();
        sm.currState = "Pattern";
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();
    }

}
