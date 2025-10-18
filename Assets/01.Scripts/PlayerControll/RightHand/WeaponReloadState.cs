// RightHand/WeaponReloadState.cs

using _01.Scripts.PlayerControll;

public class WeaponReloadState : RightHandState
{
    public WeaponReloadState(PlayerController controller, RightHandStateMachine stateMachine) : base(controller, stateMachine) { }

    public override void OnEnter()
    {
        // 재장전 시작, 타이머 설정
    }

    public override void OnUpdate()
    {
        // 재장전 완료 시 IdleState로 전환
    }
}