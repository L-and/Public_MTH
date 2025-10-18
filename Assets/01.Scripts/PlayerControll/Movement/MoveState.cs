// Movement/MoveState.cs

using _01.Scripts.PlayerControll;

public class MoveState : MovementState
{
    public MoveState(PlayerController controller, MovementStateMachine stateMachine) : base(controller, stateMachine) { }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        // 이동 입력이 없다면 IdleState로 전환
        if (controller.MoveInput.magnitude <= 0.1f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
        
        // 만약 점프 입력이 있다면 JumpState로 전환
        if (controller.PlayerInput.actions["Jump"].IsPressed() && controller.IsGrounded)
        {
            stateMachine.ChangeState(stateMachine.JumpState);
        }


        if (controller.PlayerInput.actions["Sliding"].IsPressed() && controller.IsGrounded)
        {
            stateMachine.ChangeState(stateMachine.SlidingState);
        }
    }
}