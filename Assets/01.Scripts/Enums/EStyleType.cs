namespace _01.Scripts.Enums
{
    /// <summary>
    /// 스타일의 ID를 저장하는 enum
    /// MainUI 프리팹의 StyleManager의 Style Data와 똑같이 작성해야 함
    /// </summary>
    public enum EStyleType
    {
        EnemyKill = 0,      // 처치
        BigEnemyKill = 1,   // 대형처치
        Dodge = 2,          // 회피
        Parry = 3,          // 패링
        Combo = 4,          // 일정콤보 달성
        HeadshotKill = 5    // 약점처치
    }
}
