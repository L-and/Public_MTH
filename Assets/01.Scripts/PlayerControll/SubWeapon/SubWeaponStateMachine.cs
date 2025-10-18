// SubWeapon/SubWeaponStateMachine.cs

using _01.Scripts.PlayerControll;

public class SubWeaponStateMachine : StateMachine
{
    public SubWeaponReadyState ReadyState { get; }
    public SubWeaponUsingState UsingState { get; }
    public SubWeaponCooldownState CooldownState { get; }

    public SubWeaponStateMachine(PlayerController controller)
    {
        ReadyState = new SubWeaponReadyState(controller, this);
        UsingState = new SubWeaponUsingState(controller, this);
        CooldownState = new SubWeaponCooldownState(controller, this);
    }
}
