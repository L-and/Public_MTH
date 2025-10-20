// Movement/IdleState.cs

using _01.Scripts.PlayerControll;

public class IdleState : MovementState
{
    public IdleState(PlayerController controller, MovementStateMachine stateMachine) : base(controller, stateMachine) { }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        
        // 이동상태 전환
        if (controller.MoveInput.magnitude > 0.1f)
        {
            stateMachine.ChangeState(stateMachine.MoveState);
        }

        // 점프상태 전환
        if (controller.PlayerInput.actions["Jump"].IsPressed() && controller.IsGrounded)
        {
            stateMachine.ChangeState(stateMachine.JumpState);
        }
        
        // 대쉬상태 전환
        if (controller.PlayerInput.actions["Dash"].IsPressed())
        {
            stateMachine.ChangeState(stateMachine.DashState);
        }
    }
}