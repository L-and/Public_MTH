// Movement/DashState.cs

using _01.Scripts.PlayerControll;
using UnityEngine;

public class DashState : MovementState
{
    public DashState(PlayerController controller, MovementStateMachine stateMachine) : base(controller, stateMachine) { }

    private float _elapsedTime; // DashState로 진입후 흐른 시간
    
    private float _originalDrag;
    
    public override void OnEnter()
    {
        controller.Dash();
        _elapsedTime = 0f;
        
        // 공중에서는 Drag계산 X(TODO 위치수정 필요함)
        _originalDrag = controller.Rb.linearDamping;
        controller.Rb.linearDamping = 0f;
    }

    public override void OnUpdate()
    {
        _elapsedTime += Time.deltaTime;
        
        // 입력에 따른 상태 전환
        if (controller.PlayerInput.actions["Jump"].IsPressed() && controller.IsGrounded)
        {
            stateMachine.ChangeState(stateMachine.JumpState);
        }
        
        // 슬라이딩 상태 전환
        if (controller.PlayerInput.actions["Sliding"].IsPressed() && controller.IsGrounded)
        {
            stateMachine.ChangeState(stateMachine.SlidingState);
            return;
        }
        
        // 무적시간 이후 상태 전환
        if (_elapsedTime >= controller.Status.DashDurationtime)
        {
            if (!controller.IsGrounded) // 점프상태 전환조건
            {
                stateMachine.ChangeState(stateMachine.JumpState);
            }
            else if (controller.Rb.linearVelocity.magnitude < 0.1f) // Idle상태 전환조건
            {
                stateMachine.ChangeState(stateMachine.IdleState);
            }
            else
            {
                stateMachine.ChangeState(stateMachine.MoveState);
                
            }
        }
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }


    public override void OnExit()
    {
        _elapsedTime = 0f;
        
        controller.Rb.linearDamping = _originalDrag;

    }
}