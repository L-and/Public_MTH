// Emission/EmissionStateMachine.cs

using _01.Scripts.PlayerControll;

public class EmissionStateMachine : StateMachine
{
    public EmissionReadyState ReadyState { get; }
    public EmissionUsingState UsingState { get; }
    public EmissionCooldownState CooldownState { get; }

    public EmissionStateMachine(PlayerController controller)
    {
        ReadyState = new EmissionReadyState(controller, this);
        UsingState = new EmissionUsingState(controller, this);
        CooldownState = new EmissionCooldownState(controller, this);
    }
}
