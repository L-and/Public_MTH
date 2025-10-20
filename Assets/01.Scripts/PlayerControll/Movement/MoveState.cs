// Movement/MoveState.cs

using _01.Scripts.PlayerControll;

public class MoveState : MovementState
{
    public MoveState(PlayerController controller, MovementStateMachine stateMachine) : base(controller, stateMachine) { }

    public override void OnUpdate()
    {
        // Idle상태 전환
        if (controller.MoveInput.magnitude <= 0.1f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }
        
        // 점프상태 전환
        if (controller.PlayerInput.actions["Jump"].IsPressed() && controller.IsGrounded)
        {
            stateMachine.ChangeState(stateMachine.JumpState);
            return;
        }

        // 슬라이딩 상태 전환
        if (controller.PlayerInput.actions["Sliding"].IsPressed() && controller.IsGrounded)
        {
            stateMachine.ChangeState(stateMachine.SlidingState);
            return;
        }
        
        // 대쉬상태 전환
        if (controller.PlayerInput.actions["Dash"].IsPressed())
        {
            stateMachine.ChangeState(stateMachine.DashState);
            return;
        }
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }
}