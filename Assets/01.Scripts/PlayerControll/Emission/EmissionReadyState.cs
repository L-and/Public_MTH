// Emission/EmissionReadyState.cs

using _01.Scripts.PlayerControll;

public class EmissionReadyState : EmissionState
{
    public EmissionReadyState(PlayerController controller, EmissionStateMachine stateMachine) : base(controller, stateMachine) { }

    public override void OnUpdate()
    {
        // 방출 사용 입력 감지 및 조건 충족 시 UsingState로 전환
        // 예: if (useInput && !controller.IsReloading && !controller.IsUsingSubWeapon)
    }
}