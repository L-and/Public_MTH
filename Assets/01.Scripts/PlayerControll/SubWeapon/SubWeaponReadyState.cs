// SubWeapon/SubWeaponReadyState.cs

using _01.Scripts.PlayerControll;

public class SubWeaponReadyState : SubWeaponState
{
    public SubWeaponReadyState(PlayerController controller, SubWeaponStateMachine stateMachine) : base(controller, stateMachine) { }

    public override void OnUpdate()
    {
        // 보조무기 사용 입력 감지 및 조건 충족 시 UsingState로 전환
        // 예: if (useInput && !controller.IsReloading && !controller.IsUsingEmission)
    }
}