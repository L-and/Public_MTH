// Emission/EmissionState.cs

using _01.Scripts.PlayerControll;

public abstract class EmissionState : IState
{
    protected readonly PlayerController controller;
    protected readonly EmissionStateMachine stateMachine;

    # region 일반적인 상태전환 조건 프로퍼티

    protected bool CanReady => controller.Stat.overheat.Value >= controller.CurrentEmission.overheatCost;
    protected bool CanUsing => controller.PlayerInput.actions["Emission"].WasPressedThisFrame() && 
                               controller.Stat.overheat.TryDecrease(controller.CurrentEmission.overheatCost) &&
                               !controller.IsUsingSubWeapon;
    
    # endregion
    
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
