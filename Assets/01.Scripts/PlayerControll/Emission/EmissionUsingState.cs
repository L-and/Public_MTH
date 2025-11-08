// Emission/EmissionUsingState.cs

using _01.Scripts.PlayerControll;

// TODO SMB한테 다 맡길꺼면 왜 FSM 만들었지??

/// <summary>
/// 애니메이터의 "Layer Emission"의 Default State에서 SMB를 사용해서 아래의 처리를 함
// 방출의 발사: 선택된 방출의 기능코드 호출
// 방출의 사용종료: CooldownState로 전환해 줌
/// </summary>
public class EmissionUsingState : EmissionState
{
    public EmissionUsingState(PlayerController controller, EmissionStateMachine stateMachine) : base(controller, stateMachine) { }

    public override void OnEnter()
    {
        controller.CharacterAnimController.EmissionFireAnimation();
        controller.playerEmission.ModelSetActive(true);
        
        // TODO 방출 애니메이션(무기들기, 무기내리기)에 맞게 방출사용 및 상태전환 되도록 추가
        // controller.UseEmission();
    }
}