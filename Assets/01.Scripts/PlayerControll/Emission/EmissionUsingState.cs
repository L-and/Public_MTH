// Emission/EmissionUsingState.cs

using _01.Scripts.PlayerControll;

public class EmissionUsingState : EmissionState
{
    public EmissionUsingState(PlayerController controller, EmissionStateMachine stateMachine) : base(controller, stateMachine) { }

    public override void OnEnter()
    {
        controller.CharacterAnimController.EmissionFireAnimation();
        // TODO 방출 애니메이션(무기들기, 무기내리기)에 맞게 방출사용 및 상태전환 되도록 추가
        // controller.UseEmission();
    }

    public override void OnUpdate()
    {
        // if (TODO 사용 종료 조건 감지 시 CooldownState로 전환)
        {
            stateMachine.ChangeState(stateMachine.CooldownState);
        }
    }
}