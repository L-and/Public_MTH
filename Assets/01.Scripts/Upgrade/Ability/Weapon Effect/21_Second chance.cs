using _01.Scripts.PlayerControll.Status;
using UnityEngine;

public class Secondchance_21 : MonoBehaviour
{
    public string EffectName => "두번째 기회";

    public void ApplyEffect(PlayerStat playerStat)
    {
        if (GameManager.PlayerManager == null || GameManager.PlayerManager.PlayerStat == null)
        {
            Debug.LogWarning("PlayerStat을 찾을 수 없어 두번째 기회 효과를 적용할 수 없습니다.");
            return;
        }

        // 부활 감시용 컴포넌트 추가 (중복 방지), <>에 hp 0 -> 풀피 되도록 하는 감지 스크립트
        //if (GameManager.PlayerManager.GetComponent<>() == null)
        //{
        //    GameManager.PlayerManager.gameObject.AddComponent<>();
        //    Debug.Log("두번째 기회 효과 적용됨! 사망 시 체력이 가득 찬 채로 부활합니다.");
        //}
        //else
        //{
        //    Debug.Log("이미 두번째 기회 효과가 적용되어 있습니다.");
        //}
    }
}
