using UnityEngine;

public class Jumping : BaseState
{
    //Convention : underbar prefix before name for variables are inheriting/coming from the base classLate
    private MovementSM _sm;
    private float _horizontalInput;
    private bool _grounded;

    //define layer in C# with bit shifting 
    private int _groundLayer = 1 << 6;

    public Jumping(MovementSM stateMachine) : base("Jumping", stateMachine)
    {
        _sm = (MovementSM)this.stateMachine;
    }

    //jump right when enter the state
    public override void Enter()
    {
        base.Enter();
        Vector2 vel = _sm.rigidbody.velocity;
        vel.y += _sm.jumpForce;
        _sm.rigidbody.velocity = vel;
        _horizontalInput = 0f;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (_grounded)
            stateMachine.ChangeState(_sm.idleState);
        _horizontalInput = Input.GetAxis("Horizontal");

        //for exercise -------------------
        if (_horizontalInput > Mathf.Epsilon)
        {
            _sm.spriteRenderer.flipX = false;
        }
        else
        {
            _sm.spriteRenderer.flipX = true;
        }
    }

    public override void UpdateLate()
    {
        base.UpdateLate();
        Vector2 vel = _sm.rigidbody.velocity;
        vel.x = _horizontalInput * ((MovementSM)stateMachine).speed;
        _sm.rigidbody.velocity = vel;
        //check grounded == true if you are falling down and touching the ground
        _grounded = _sm.rigidbody.velocity.y < Mathf.Epsilon && _sm.rigidbody.IsTouchingLayers(_groundLayer);
    }
}
