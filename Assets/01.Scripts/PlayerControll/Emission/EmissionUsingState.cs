// Emission/EmissionUsingState.cs

using _01.Scripts.PlayerControll;

public class EmissionUsingState : EmissionState
{
    public EmissionUsingState(PlayerController controller, EmissionStateMachine stateMachine) : base(controller, stateMachine) { }

    public override void OnEnter()
    {
        // 현재 선택된 방출 스킬(레일건, 화염방사기 등)의 로직 실행
    }

    public override void OnUpdate()
    {
        // 사용 종료 조건 감지 시 CooldownState로 전환
    }
}