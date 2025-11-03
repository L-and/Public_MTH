// Movement/MovementState.cs

// 모든 움직임 상태들의 부모 클래스입니다.
// PlayerController와 MovementStateMachine 참조를 공유하여 코드 중복을 줄입니다.

using _01.Scripts.PlayerControll;

public abstract class MovementState : IState
{
    protected readonly PlayerController controller;
    protected readonly MovementStateMachine stateMachine;

    # region 일반적인 상태전환 조건 프로퍼티

    protected bool CanIdle => controller.MoveInput.magnitude <= 0.1f && controller.IsGrounded;
    protected bool CanMove => controller.MoveInput.magnitude > 0.1f && controller.IsGrounded;
    protected bool CanJump => controller.PlayerInput.actions["Jump"].WasPressedThisFrame() && controller.IsGrounded;
    protected bool CanDash => controller.PlayerInput.actions["Dash"].WasPressedThisFrame() && controller.Stat.stamina.TryDecrease(controller.DashCost);
    protected bool CanSlide => controller.PlayerInput.actions["Sliding"].IsPressed() && controller.IsGrounded;
    
    # endregion
    
    protected MovementState(PlayerController controller, MovementStateMachine stateMachine)
    {
        this.controller = controller;
        this.stateMachine = stateMachine;
    }

    public virtual void OnEnter() { }

    public virtual void OnUpdate() { }

    public virtual void OnFixedUpdate()
    {
        controller.MovePlayer(controller.CurrentAcceleration);
        controller.LimitSpeed();
    }
    public virtual void OnLateUpdate() { }
    public virtual void OnExit() { }
}
