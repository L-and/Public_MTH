// Movement/SlidingState.cs

using _01.Scripts.PlayerControll;
using UnityEngine;

public class SlidingState : MovementState
{
    public SlidingState(PlayerController controller, MovementStateMachine stateMachine) : base(controller, stateMachine) { }

    private float _slidingTIme = 0.3f; // 슬라이딩상태 지속시간
    
    private float _elapsedTime; // 상태 진입 후 흐른 시간

    private float _originalColliderHeight;
    private readonly float _slidingColliderHeight = 1f;
    
    public override void OnEnter()
    {
        _elapsedTime = 0f; //상태 시간 초기화

        // 콜라이더 높이 변경
        _originalColliderHeight = controller.CapsuleCollider.height;
        controller.CapsuleCollider.height = _slidingColliderHeight;
        
        controller.Rb.AddForce(-controller.transform.up * 5f, ForceMode.Impulse);
    }

    public override void OnUpdate()
    {
        _elapsedTime += Time.deltaTime;
        
        // 점프상태 전환
        if (controller.PlayerInput.actions["Jump"].IsPressed() && controller.IsGrounded)
        {
            stateMachine.ChangeState(stateMachine.JumpState);
            return;
        }
        
        // 대쉬상태 전환
        if (controller.PlayerInput.actions["Dash"].IsPressed())
        {
            stateMachine.ChangeState(stateMachine.DashState);
            return;
        }
        
        // 슬라이딩버튼을 때면 Idle상태로 전환
        if (!controller.PlayerInput.actions["Sliding"].IsPressed())
            stateMachine.ChangeState(stateMachine.IdleState);
    }

    public override void OnFixedUpdate()
    {
        if (_elapsedTime <= _slidingTIme)
        {
            controller.Sliding(); // 슬라이딩 물리적용
            controller.LimitSpeed();
        }
    }
    

    public override void OnExit()
    {
        _elapsedTime = 0f; //상태 시간 초기화
        // 콜라이더 높이 복구
        controller.CapsuleCollider.height = _originalColliderHeight;
    }
}