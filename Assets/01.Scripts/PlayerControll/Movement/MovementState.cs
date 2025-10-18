// Movement/MovementState.cs

// 모든 움직임 상태들의 부모 클래스입니다.
// PlayerController와 MovementStateMachine 참조를 공유하여 코드 중복을 줄입니다.

using _01.Scripts.PlayerControll;

public abstract class MovementState : IState
{
    protected readonly PlayerController controller;
    protected readonly MovementStateMachine stateMachine;

    protected MovementState(PlayerController controller, MovementStateMachine stateMachine)
    {
        this.controller = controller;
        this.stateMachine = stateMachine;
    }

    public virtual void OnEnter() { }

    public virtual void OnUpdate()
    {
        controller.MovePlayer(controller.Status.CurrentAcceleration);
        controller.LimitSpeed();
    }

    public virtual void OnFixedUpdate()
    {
 
        
    }
    public virtual void OnLateUpdate() { }
    public virtual void OnExit() { }
}
