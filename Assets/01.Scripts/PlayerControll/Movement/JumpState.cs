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
        if (CanDash)
        {
            stateMachine.ChangeState(stateMachine.DashState);
            return;
        }

        if (CanMove)
        {
            stateMachine.ChangeState(stateMachine.MoveState);
            return;
        }
        
        // Idle상태 전환
        if (CanIdle)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }
    }

    public override void OnExit()
    {
        controller.Rb.linearDamping = _originalDrag;
    }
}