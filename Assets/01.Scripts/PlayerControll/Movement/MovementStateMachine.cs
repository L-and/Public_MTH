// Movement/MovementStateMachine.cs

// 움직임 상태들을 관리하는 주체입니다.

using _01.Scripts.PlayerControll;

public class MovementStateMachine : StateMachine
{
    // 각 상태에 대한 인스턴스를 소유하여 언제든 접근할 수 있도록 합니다.
    public IdleState IdleState { get; }
    public MoveState MoveState { get; }
    public CrouchState CrouchState { get; }
    public SlidingState SlidingState { get; }
    public DashState DashState { get; }
    public JumpState JumpState { get; }

    public MovementStateMachine(PlayerController controller)
    {
        // 상태 인스턴스 생성
        IdleState = new IdleState(controller, this);
        MoveState = new MoveState(controller, this);
        CrouchState = new CrouchState(controller, this);
        SlidingState = new SlidingState(controller, this);
        DashState = new DashState(controller, this);
        JumpState = new JumpState(controller, this);
    }
}
