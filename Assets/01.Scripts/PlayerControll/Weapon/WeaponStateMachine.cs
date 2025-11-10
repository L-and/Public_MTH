using _01.Scripts.PlayerControll;

public class WeaponStateMachine : StateMachine
{
    public WeaponIdleState IdleState { get; }
    public WeaponFireState FireState { get; }
    public WeaponReloadState ReloadState { get; }

    public WeaponStateMachine(PlayerController controller)
    {
        IdleState = new WeaponIdleState(controller, this);
        FireState = new WeaponFireState(controller, this);
        ReloadState = new WeaponReloadState(controller, this);
    }
}
