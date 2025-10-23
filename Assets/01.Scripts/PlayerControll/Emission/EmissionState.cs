// Emission/EmissionState.cs

using _01.Scripts.PlayerControll;

public abstract class EmissionState : IState
{
    protected readonly PlayerController controller;
    protected readonly EmissionStateMachine stateMachine;

    protected EmissionState(PlayerController controller, EmissionStateMachine stateMachine)
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
