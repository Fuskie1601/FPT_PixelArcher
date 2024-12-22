//Blueprint for other state to inherit so all method is virtual

//For reference : virtual and abstract are used in the base class. virtual means, use the extended class's version
//if it has one otherwise use this base class's version. abstract means the extended class MUST have a new version.


using UnityEngine;

public class BaseState
{
    //for identification
    public string name;
    //protected so only accessible in this class and it children
    protected StateMachine stateMachine;
    //Constructor
    public BaseState(string name, StateMachine stateMachine)
    {
        this.name = name;
        this.stateMachine = stateMachine;
    }

    //Enter() is call Start() in Unity life cycle
    public virtual void Enter() { }
    //UpdateLogic() is call Update() in Unity life cycle
    public virtual void UpdateLogic() { }
    //UpdateLate() is call LateUpdate() in Unity Life cycle
    public virtual void UpdateLate() { }
    //UpdateFixed() is call FixedUpdate() in Unity Life cycle
    public virtual void UpdateFixed() { }
    //Exit() is call before changing to new state
    public virtual void Exit() { }
    //TriggerEnter() is call in OnTriggerEnter()
    public virtual void TriggerEnter(Collider other) { }
    //TriggerExit() is call in OnTriggerExit()
    public virtual void TriggerExit(Collider other) { }
}
