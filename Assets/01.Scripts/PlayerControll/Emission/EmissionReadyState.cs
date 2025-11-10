// Emission/EmissionReadyState.cs

using _01.Scripts.PlayerControll;

public class EmissionReadyState : EmissionState
{
    public EmissionReadyState(PlayerController controller, EmissionStateMachine stateMachine) : base(controller, stateMachine) { }

    public override void OnUpdate()
    {
        if (controller.equippedWeapon?.WeaponState != EPlayerStates.WeaponState.Reload && 
            CanUsing)
        {
            stateMachine.ChangeState(stateMachine.UsingState);
        }
        
    }
}