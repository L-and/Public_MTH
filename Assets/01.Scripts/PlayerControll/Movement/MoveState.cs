// Movement/MoveState.cs

using _01.Scripts.PlayerControll;

public class MoveState : MovementState
{
    public MoveState(PlayerController controller, MovementStateMachine stateMachine) : base(controller, stateMachine) { }

    public override void OnUpdate()
    {
        // Idle상태 전환
        if (CanIdle)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }
        
        // 점프상태 전환
        if (CanJump)
        {
            stateMachine.ChangeState(stateMachine.JumpState);
            return;
        }

        // 슬라이딩 상태 전환
        if (CanSlide)
        {
            stateMachine.ChangeState(stateMachine.SlidingState);
            return;
        }
        
        // 대쉬상태 전환
        if (CanDash)
        {
            stateMachine.ChangeState(stateMachine.DashState);
            return;
        }
    }
}