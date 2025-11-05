using _01.Scripts.PlayerControll.Status;
using System.Linq;
using UnityEngine;

public class ElevatorUpgradeManager : MonoBehaviour
{
    [Header("참조")] 
    public UpgradeData upgradeData;
    public elevtest3 linkedElevator;
    public UpgradeUIManager upgradeUIManager; //

    // PlayerManager를 통해 PlayerStat을 참조하는 프로퍼티
    private PlayerStat PlayerStat => GameManager.PlayerManager?.PlayerStat;
    
    private IUpgradeEffect[] allEffects;
    private bool effectsLoaded = false;

    private int currentFloor = 0;     // 🌟 현재 층
    private const int maxFloor = 3;   // 🌟 총 3층 기준 (필요시 조정 가능)

    private void Awake()
    {
        LoadAllEffects();
        //var monoBehaviours = Resources.FindObjectsOfTypeAll<MonoBehaviour>();
        //allEffects = monoBehaviours.OfType<IUpgradeEffect>().ToArray();
    }

    private void LoadAllEffects()
    {
        // 비활성 포함 전체 탐색
        var monoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        allEffects = monoBehaviours.OfType<IUpgradeEffect>().ToArray();
        effectsLoaded = allEffects.Length > 0;

        //감지된 효과 목록 로그 출력
        if (allEffects.Length > 0)
        {
            string detectedEffects = string.Join(", ", allEffects.Select(e => e.EffectName));
            Debug.Log($"[ElevatorUpgradeManager] 감지된 업그레이드 효과 ({allEffects.Length}개): {detectedEffects}");
        }
        else
        {
            Debug.Log("[ElevatorUpgradeManager] 감지된 업그레이드 효과가 없습니다!");
        }
    }

 
    //  적용효과 목록 로그 출력
    public void ApplyUpgrade(int id)
    {
        Debug.Log($"[DEBUG] 업그레이드 버튼 클릭됨 ID: {id}");

        //PlayerStatus의 UpgradeSlotID에 id 값 할당
        int[] upgradeSlots = new int[3] { PlayerStat.UpgradeSlot1ID, PlayerStat.UpgradeSlot2ID, PlayerStat.UpgradeSlot3ID };
        upgradeSlots[PlayerStat.UpgradeNumber] = id;
        PlayerStat.UpgradeNumber++;
        Debug.Log(PlayerStat.UpgradeSlot1ID);

        //아직 효과가 로드 안되면 즉시 다시 시도
        if (!effectsLoaded || allEffects == null || allEffects.Length == 0)
        {
            Debug.Log("[ElevatorUpgradeManager]  효과 리스트가 비어 있음. 다시 로드 시도 중...");
            LoadAllEffects();
        }

        if (!upgradeData.Upgrade.ContainsKey(id))
        {
            Debug.Log($" ID {id} 업그레이드가 존재하지 않습니다.");
            return;
        }

        // UpgradeData에서 직접 스크립트 이름 가져오기
        string effectName = upgradeData.Upgrade[id].Effect;
        Debug.Log($"[DEBUG] '{effectName}' 효과 적용 시도 중....");
        var effect = allEffects.FirstOrDefault(e => e.EffectName == effectName); // 정확한 이름 맞는지 비교

        if (effect != null)
        {
            effect.ApplyEffect(PlayerStat);
            Debug.Log($" {effectName} 업그레이드 적용 완료");
        }
        else
        {
            Debug.Log($" {effectName} 효과 스크립트를 찾지 못했습니다. (씬에 존재하지 않음)");
        }
    }
}
