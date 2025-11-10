// PlayerStates.cs
// 모든 상태 타입을 enum으로 중앙에서 관리하여 가독성과 접근성을 높입니다.
namespace _01.Scripts.PlayerControll
{
    public static class EPlayerStates
    {
        public enum MovementState
        {
            Idle,
            Move,
            Crouch,
            Sliding,
            Dash,
            Jump
        }

        public enum WeaponState
        {
            Idle,
            Fire,
            Reload
        }

        // 보조무기 상태
        public enum SubWeaponState
        {
            Ready,
            Using,
            Cooldown
        }

        // 방출 상태
        public enum EmissionState
        {
            Ready,
            Using,
            Cooldown
        }
    }
}