// Movement/IdleState.cs

using _01.Scripts.PlayerControll;

public class IdleState : MovementState
{
    public IdleState(PlayerController controller, MovementStateMachine stateMachine) : base(controller, stateMachine) { }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        
        // 만약 이동 입력이 있다면 MoveState로 전환
        if (controller.MoveInput.magnitude > 0.1f)
        {
            stateMachine.ChangeState(stateMachine.MoveState);
        }

        // 만약 점프 입력이 있다면 JumpState로 전환
        if (controller.PlayerInput.actions["Jump"].IsPressed() && controller.IsGrounded)
        {
            stateMachine.ChangeState(stateMachine.JumpState);
        }
    }
}