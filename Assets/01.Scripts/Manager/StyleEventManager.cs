using System;
using _01.Scripts.Enums;
using UnityEngine;

/// <summary>
/// 스타일리쉬 액션이 발생할 때 실행해야 할 이벤트를 관리하는 스크립트
/// </summary>
public static class StyleEventManager
{
    // styleData에 정의된 스타일의 ID
    public static event Action<int> OnStyleAction;

    public static void TriggerStyleAction(EStyleType styleId)
    {
        // 스타일리쉬 액션관련 과열게이지 증가
        
        if (styleId == EStyleType.SlidingUpgrade)
        {
            GameManager.PlayerManager?.PlayerStat.AddOverHeat(2);
        }
        
        Debug.Log($"[스타일리쉬 액션 이벤트] {styleId} 실행");
        OnStyleAction?.Invoke((int)styleId);
    }
}
