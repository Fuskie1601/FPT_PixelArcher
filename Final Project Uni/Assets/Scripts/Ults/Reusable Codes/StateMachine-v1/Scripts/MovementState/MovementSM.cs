using UnityEngine;

public class MovementSM : StateMachine
{
    public float speed = 4f;
    public float jumpForce = 30f;
    public Rigidbody2D rigidbody;
    public SpriteRenderer spriteRenderer;

    //Declare all states included
    [HideInInspector]
    public Idle idleState;
    [HideInInspector]
    public Moving movingState;
    [HideInInspector]
    public Jumping jumpingState;

    //Initialize all states included and putting this SM as parameter
    private void Awake()
    {
        idleState = new Idle(this);
        movingState = new Moving(this);
        jumpingState = new Jumping(this);
    }

    //Set initial state
    protected override BaseState GetInitialState()
    {
        return idleState;
    }
}
