using UnityEngine;

public class Grounded : BaseState
{   
    //Save the current SM to work on it later
    protected MovementSM sm;

    //Constructor , derive from BaseState Constructor 
    //pass in name and State machine that use this state
    //because child state derive from this , smallest child need 
    //to provide specific name but parent state not 
    public Grounded(string name, MovementSM stateMachine) : base(name, stateMachine)
    {
        sm = (MovementSM) this.stateMachine;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        //Check for jump to transition to jumping state
        if (Input.GetKeyDown(KeyCode.Space))
            stateMachine.ChangeState(sm.jumpingState);
    }

}