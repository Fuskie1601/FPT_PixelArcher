HIERARCHICAL FINITE STATE MACHINE PATTERN

+CREDIT:
    -Modified State Machine by Mina Pêcheux (yt:https://www.youtube.com/@minapecheux)
    -Link to original : https://github.com/MinaPecheux/UnityTutorials-FiniteStateMachines

+OVERVIEW:
    -Manage States and their transition
    -Check code and comment for implementation detail

+INSTRUCTION:
    -Inherit the StateMachine to create a specific SM
    -Create States for the specific SM by inherit BaseState
    -Declare all the states needed inside the specific SM
    -Set initial state inside GetInitialState() method
    -Set the specific SM as a component of an GameObject if needed 
    -Put logic inside appropriate method in appropriate state

+METHOD OVERVIEW:
    -State:
        -Enter() is call in Start() in Unity life cycle
        -UpdateLogic() is call in Update() in Unity life cycle
        -UpdateLate() is call LateUpdate() in Unity Life cycle
        -UpdateFixed() is call FixedUpdate() in Unity Life cycle
        -Exit() is call before changing to new state
        -TriggerEnter() is call when collision detected
    -State Machine:
        -GetInitialState() get the initial state of this machine

+TO-DO:
    -Add more functionality when needed in the future

+CHANGE LOGS:
    -9/1/2024 : Created - Q
    -17/1/2024 : Implemented TriggerEnter() - Q

+MORE RESOURCE:
    -Original video tutorial:
        -https://www.youtube.com/watch?v=-VkezxxjsSE
        -https://www.youtube.com/watch?v=OtUKsjPWzO8
    -State Machine pattern overview : 
        -https://www.gameprogrammingpatterns.com/state.html