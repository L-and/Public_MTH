// SubWeapon/SubWeaponCooldownState.cs

using _01.Scripts.PlayerControll;

public class SubWeaponCooldownState : SubWeaponState
{
    public SubWeaponCooldownState(PlayerController controller, SubWeaponStateMachine stateMachine) : base(controller, stateMachine) { }

    public override void OnEnter()
    {
        // 쿨타임 타이머 시작
    }

    public override void OnUpdate()
    {
        // 쿨타임 종료 시 ReadyState로 전환
    }
}