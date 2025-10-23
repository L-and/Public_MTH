// Movement/JumpState.cs

using _01.Scripts.PlayerControll;

public class JumpState : MovementState
{
    public JumpState(PlayerController controller, MovementStateMachine stateMachine) : base(controller, stateMachine) { }

    private float _originalDrag;
    
    public override void OnEnter()
    {
        controller.Jump(); // 점프 물리처리
        controller.isMovementStateLocked = true;
        controller.UnlockMovementStateLock(0.2f); // 점프이후 일정시간 지면검사 무시
        
        // 공중에서는 Drag계산 X(TODO 위치수정 필요함)
        _originalDrag = controller.Rb.linearDamping;
        controller.Rb.linearDamping = 0f;
    }

    public override void OnUpdate()
    {
        if (controller.isMovementStateLocked) return;
        
        // 대쉬상태 전환
        if (controller.PlayerInput.actions["Dash"].IsPressed())
        {
            stateMachine.ChangeState(stateMachine.DashState);
        }
        
        // 지면에 닿으면 Idle상태 전환
        if (controller.IsGrounded)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    public override void OnExit()
    {
        controller.Rb.linearDamping = _originalDrag;
    }
}