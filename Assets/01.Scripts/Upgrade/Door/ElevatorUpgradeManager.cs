using _01.Scripts.PlayerControll.Status;
using DG.Tweening;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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

        SetUpgradeSlot();

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
        //string effectName = upgradeData.Upgrade[id].Effect;
        //Debug.Log($"[DEBUG] '{effectName}' 효과 적용 시도 중....");
        //var effect = allEffects.FirstOrDefault(e => e.EffectName == effectName); // 정확한 이름 맞는지 비교

        //if (effect != null)
        //{
        //    effect.ApplyEffect(PlayerStat);
        //    Debug.Log($" {effectName} 업그레이드 적용 완료");
        //}
        //else
        //{
        //    Debug.Log($" {effectName} 효과 스크립트를 찾지 못했습니다. (씬에 존재하지 않음)");
        //}

        // UpgradePersistence로 일원화
        if (UpgradePersistence.Instance != null)
        {
            UpgradePersistence.Instance.SaveAndApply(id, PlayerStat);
            GameManager.SceneEx.LoadScene(Constants.GAMESCENE);
            Debug.Log($"[ElevatorUpgradeManager] UpgradePersistence에 ID {id} 저장 및 적용 요청 완료");
        }
        else
        {
            Debug.LogWarning("[ElevatorUpgradeManager] UpgradePersistence 인스턴스를 찾지 못했습니다!");
        }
    }

    public void SetUpgradeSlot()
    {
        Image upgradeSlot1 = GameObject.Find("Upgrade Slot 1").GetComponent<Image>();
        Image upgradeSlot2 = GameObject.Find("Upgrade Slot 2").GetComponent<Image>();
        Image upgradeSlot3 = GameObject.Find("Upgrade Slot 3").GetComponent<Image>();
        Image[] upgradeSlots = new Image[3] { upgradeSlot1, upgradeSlot2, upgradeSlot3 };
        int[] upgradeSlotIDs = new int[3] { GameManager.PlayerManager.PlayerStat.UpgradeSlot1ID, GameManager.PlayerManager.PlayerStat.UpgradeSlot2ID, GameManager.PlayerManager.PlayerStat.UpgradeSlot3ID };

        for (int i = 0; i < upgradeSlots.Length; i++)
        {
            if (i <= GameManager.PlayerManager.PlayerStat.UpgradeNumber - 1)
            {
                int index = upgradeSlotIDs[i];
                upgradeSlots[i].DOFade(1, 0);
                upgradeSlots[i].sprite = upgradeData.Upgrade[index].Icon;
            }
            else
            {
                upgradeSlots[i].DOFade(0, 0);
            }
        }
    }
}
