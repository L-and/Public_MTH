// Emission/EmissionReadyState.cs

using _01.Scripts.PlayerControll;

public class EmissionReadyState : EmissionState
{
    public EmissionReadyState(PlayerController controller, EmissionStateMachine stateMachine) : base(controller, stateMachine) { }

    public override void OnUpdate()
    {
        if (CanUsing)
        {
            stateMachine.ChangeState(stateMachine.UsingState);
        }
        
    }
}