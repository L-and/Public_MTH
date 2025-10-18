// RightHand/WeaponIdleState.cs

using _01.Scripts.PlayerControll;

public class WeaponIdleState : RightHandState
{
    public WeaponIdleState(PlayerController controller, RightHandStateMachine stateMachine) : base(controller, stateMachine) { }

    public override void OnUpdate()
    {
        // 발사 입력 감지 시 FireState로 전환
        // 재장전 입력 감지 및 조건 충족 시 ReloadState로 전환
    }
}