// SubWeapon/SubWeaponUsingState.cs

using _01.Scripts.PlayerControll;

public class SubWeaponUsingState : SubWeaponState
{
    public SubWeaponUsingState(PlayerController controller, SubWeaponStateMachine stateMachine) : base(controller, stateMachine) { }

    public override void OnEnter()
    {
        // 현재 선택된 보조무기(방패, 그랩 등)의 로직 실행
    }

    public override void OnUpdate()
    {
        // 사용 종료 조건 감지 시 CooldownState로 전환
    }
}