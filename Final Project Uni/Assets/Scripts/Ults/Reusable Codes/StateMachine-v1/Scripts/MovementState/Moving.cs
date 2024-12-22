using UnityEngine;

//inherit from parent state Grounded
public class Moving : Grounded
{
    private float _horizontalInput;

    public Moving(MovementSM stateMachine) : base("Moving", stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        _horizontalInput = 0f;
    }

    //Inheritance note : First run UpdateLogic() from BaseState then from Grounded then from Moving 
    //Highest to lowest in hierarchy of inheritance
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        _horizontalInput = Input.GetAxis("Horizontal");
        if (Mathf.Abs(_horizontalInput) < Mathf.Epsilon)
        {
            stateMachine.ChangeState(sm.idleState);
        }
        //for exercise -------------------
        if (_horizontalInput > Mathf.Epsilon)
        {
            sm.spriteRenderer.flipX = false;
        }
        else
        {
            sm.spriteRenderer.flipX = true;
        }
        //--------------------------------
    }

    public override void UpdateLate()
    {
        base.UpdateLate();
        Vector2 vel = sm.rigidbody.velocity;
        vel.x = _horizontalInput * ((MovementSM)stateMachine).speed;
        sm.rigidbody.velocity = vel;
    }
}
