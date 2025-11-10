// Emission/EmissionCooldownState.cs

using _01.Scripts.PlayerControll;

public class EmissionCooldownState : EmissionState
{
    public EmissionCooldownState(PlayerController controller, EmissionStateMachine stateMachine) : base(controller, stateMachine) { }

    public override void OnEnter()
    {
    }

    public override void OnUpdate()
    {
        if (CanReady)
        {
            stateMachine.ChangeState(stateMachine.ReadyState);
        }
    }
}