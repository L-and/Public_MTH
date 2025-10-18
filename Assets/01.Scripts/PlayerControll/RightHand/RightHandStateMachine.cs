// RightHand/RightHandStateMachine.cs

using _01.Scripts.PlayerControll;

public class RightHandStateMachine : StateMachine
{
    public WeaponIdleState IdleState { get; }
    public WeaponFireState FireState { get; }
    public WeaponReloadState ReloadState { get; }

    public RightHandStateMachine(PlayerController controller)
    {
        IdleState = new WeaponIdleState(controller, this);
        FireState = new WeaponFireState(controller, this);
        ReloadState = new WeaponReloadState(controller, this);
    }
}
