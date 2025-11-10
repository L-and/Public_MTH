// Movement/IdleState.cs

using _01.Scripts.PlayerControll;

public class IdleState : MovementState
{
    public IdleState(PlayerController controller, MovementStateMachine stateMachine) : base(controller, stateMachine) { }
    
    public override void OnUpdate()
    {
        // 이동상태 전환
        if (CanMove)
        {
            stateMachine.ChangeState(stateMachine.MoveState);
            return;
        }

        // 점프상태 전환
        if (CanJump)
        {
            stateMachine.ChangeState(stateMachine.JumpState);
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