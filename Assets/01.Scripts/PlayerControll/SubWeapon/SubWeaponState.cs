// SubWeapon/SubWeaponState.cs

using _01.Scripts.PlayerControll;

public abstract class SubWeaponState : IState
{
    protected readonly PlayerController controller;
    protected readonly SubWeaponStateMachine stateMachine;

    protected SubWeaponState(PlayerController controller, SubWeaponStateMachine stateMachine)
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
