using _01.Scripts.PlayerControll;

public abstract class WeaponState : IState
{
    protected readonly PlayerController controller;
    protected readonly WeaponStateMachine stateMachine;

    protected WeaponState(PlayerController controller, WeaponStateMachine stateMachine)
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
