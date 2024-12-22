//Abstract State machine , inherit to make a specific state machine 
//Only deal with running a state logic and it transition
//Implement the Logic in a state 
//Inherit SM need to have all of it states declare
//and set a return state in GetInitialState()

using UnityEngine;

public class StateMachine : MonoBehaviour
{

    public BaseState currentState;

    //IN-CASE : need to remember previous state (uncomment in ChangeState() too )
    //BaseState previousState;

    //Enter initial state when start the state machine
    void Start()
    {
        currentState = GetInitialState();
        if (currentState != null)
            currentState.Enter();
    }

    //Run state's UpdateLogic()
    void Update()
    {
        if (currentState != null)
            currentState.UpdateLogic();
    }

    //Run state's UpdateLate()
    void LateUpdate()
    {
        if (currentState != null)
            currentState.UpdateLate();
    }

    //Run state's UpdateFixed()
    void FixedUpdate()
    {
        if (currentState != null)
        {
            currentState.UpdateFixed();
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (currentState != null)
        {
            currentState.TriggerEnter(other);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (currentState != null)
        {
            currentState.TriggerExit(other);
        }
    }

    //return a initial state in inherited SM (i.e : IdleState)
    protected virtual BaseState GetInitialState()
    {
        return null;
    }

    //State transition method , run current state Exit() and new state Enter()
    public void ChangeState(BaseState newState)
    {
        //IN-CASE : need to remember previous state
        //previousState = currentState;
        
        currentState.Exit();

        currentState = newState;
        newState.Enter();
    }


    //Only for Debugging , show state name on screen
    // private void OnGUI()
    // {
    //     GUILayout.BeginArea(new Rect(10f, 10f, 200f, 100f));
    //     string content = currentState != null ? currentState.name : "(no current state)";
    //     GUILayout.Label($"<color='black'><size=40>{content}</size></color>");
    //     GUILayout.EndArea();
    // }
}
