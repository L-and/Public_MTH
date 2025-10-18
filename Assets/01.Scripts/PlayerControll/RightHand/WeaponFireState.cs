// RightHand/WeaponFireState.cs

using _01.Scripts.PlayerControll;

public class WeaponFireState : RightHandState
{
    public WeaponFireState(PlayerController controller, RightHandStateMachine stateMachine) : base(controller, stateMachine) { }

    public override void OnEnter()
    {
        // 무기 발사 로직 (Raycast, 총알 생성 등)
    }
}