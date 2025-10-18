// RightHand/RightHandState.cs

using _01.Scripts.PlayerControll;

public abstract class RightHandState : IState
{
    protected readonly PlayerController controller;
    protected readonly RightHandStateMachine stateMachine;

    protected RightHandState(PlayerController controller, RightHandStateMachine stateMachine)
    {
        this.controller = controller;
        this.stateMachine = stateMachine;
    }

    public virtual void OnEnter() { }
    public virtual void OnUpdate() { }
    public virtual void OnFixedUpdate() { }
    public virtual void OnLateUpdate() { }
    public virtual void OnExit() { }
}
