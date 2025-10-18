// Movement/SlidingState.cs

using _01.Scripts.PlayerControll;
using UnityEngine;

public class SlidingState : MovementState
{
    public SlidingState(PlayerController controller, MovementStateMachine stateMachine) : base(controller, stateMachine) { }

    public override void OnEnter()
    {
        controller.Sliding();
        controller.CapsuleCollider.height = 1f;
        controller.Rb.AddForce(-controller.transform.up * 10f, ForceMode.Impulse);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        // 만약 점프 입력이 있다면 JumpState로 전환
        if (controller.PlayerInput.actions["Jump"].IsPressed())
        {
                Debug.Log(controller.IsGrounded);

            stateMachine.ChangeState(stateMachine.JumpState);
        }
        
        // 슬라이딩버튼을 때면 Idle상태로 전환
        if (!controller.PlayerInput.actions["Sliding"].IsPressed())
            stateMachine.ChangeState(stateMachine.IdleState);
    }

    public override void OnExit()
    {
        controller.CapsuleCollider.height = 2f;
    }
}