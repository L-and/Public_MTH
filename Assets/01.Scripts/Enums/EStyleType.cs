namespace _01.Scripts.Enums
{
    /// <summary>
    /// 스타일의 ID를 저장하는 enum
    /// MainUI 프리팹의 StyleManager의 Style Data와 똑같이 작성해야 함
    /// </summary>
    public enum EStyleType
    {
        // TODO 대형 적 처치를 구분하는 로직이 추가되면 EnemyKill 사용부분을 변경해야 함
        EnemyKill = 0,      // 처치
        BigEnemyKill = 1,   // 대형처치
        Dodge = 2,          // 회피
        Parry = 3,          // 패링
        Combo = 4,          // 일정콤보 달성
        SlidingUpgrade = 5, // 슬라이딩중 과열충전
        ParryKill = 6,      // 패링된 투사체로 적 처치
        RailgunKill = 7,    // 레일건으로 적처치
        MultiKill = 8,      // 동시처치
        HeadshotKill = 9,    // 약점처치
    }
}
