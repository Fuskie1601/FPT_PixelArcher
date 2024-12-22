using Mono.CSharp;
using UnityEngine;

public class BotChase : BotTargeting
{

    public BotChase(BotSM stateMachine) : base("Chase", stateMachine)
    {

    }
    public override void Enter()
    {
        base.Enter();
        sm.currState = "Chase";
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (Vector3.Distance(TF.position, sm.destination) > 1)
        {
            sm.agent.SetDestination(sm.destination);
            
            if(sm.bot._animController != null)
            sm.bot._animController.UpdateRunInput(true);
        }
        else
        {
            if(sm.bot._animController != null)
            sm.bot._animController.UpdateRunInput(false);
            
            sm.agent.isStopped = true;
            sm.ChangeState(sm.AttackingState);
        }
    }

}
