using UnityEngine;

//inherit from parent state Grounded
public class Idle : Grounded
{
    private float _horizontalInput;

    //Constructor , derive from Grounded state 
    // put "Idle" as name and MovementSM as state machine
    public Idle (MovementSM stateMachine) : base("Idle", stateMachine) {}

    //override BaseState Enter() but still run the BaseState Enter() logic
    public override void Enter()
    {
        base.Enter();
        _horizontalInput = 0f;
    }

    //override BaseState UpdateLogic() but still run the BaseState UpdateLogic() logic
    
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        _horizontalInput = Input.GetAxis("Horizontal");
        //Change to moving state if input > 0
        //Mathf.Epsilon for estimation of 0 in float
        if (Mathf.Abs(_horizontalInput) > Mathf.Epsilon)
            stateMachine.ChangeState(sm.movingState);
    }
}