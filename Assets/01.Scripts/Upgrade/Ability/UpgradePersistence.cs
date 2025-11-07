using _01.Scripts.PlayerControll.Status;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// 선택된 업그레이드 id를 보관, 씬 로드시 playerStat이 준비 시, 자동 재적용

public class UpgradePersistence : MonoBehaviour
{
    public static UpgradePersistence Instance { get; private set; }

    [Header("데이터 참조")]
    public UpgradeData upgradeData; // 기존 UpgradeData를 인스펙터에 연결

    [Header("효과 스크립트 호스트")]
    public Transform effectsHost; //  WeaponEffect (전체 효과 컴포넌트) 연결

    // 씬을 넘어 유지할 '선택된 업그레이드 ID' 보관소
    private readonly List<int> selectedUpgradeIds = new List<int>();

    // 캐시된 효과 사전: EffectName -> IUpgradeEffect
    private Dictionary<string, IUpgradeEffect> effectMap;

    private void Awake()
    {
        // 싱글톤 & 유지
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 효과 맵 구성
        BuildEffectMap();

        // 씬 로드 감지 → PlayerStat 준비되면 재적용
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void BuildEffectMap()
    {
        if (effectsHost == null)
        {
            Debug.LogWarning("[UpgradePersistence] effectsHost가 비어있습니다. 자식에 IUpgradeEffect들을 붙여주세요.");
            effectMap = new Dictionary<string, IUpgradeEffect>();
            return;
        }

        var effects = effectsHost.GetComponentsInChildren<IUpgradeEffect>(true);
        effectMap = effects.GroupBy(e => e.EffectName)
                           .ToDictionary(g => g.Key, g => g.First());

        Debug.Log($"[UpgradePersistence] 효과 로드 완료: {effectMap.Count}개 ({string.Join(", ", effectMap.Keys)})");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 바뀔 때마다 재적용 시도
        StartCoroutine(ReapplyWhenPlayerReady());


    }

    private IEnumerator ReapplyWhenPlayerReady()
    {
        // GameManager를 건드릴 수 없으므로, 준비될 때까지 기다렸다가 가져옵니다.
        // PlayerStat 핸들이 생길 때까지 폴링
        PlayerStat playerStat = null;

        // 타임아웃을 너무 짧게 두면 초기 씬 로딩 타이밍에 놓칠 수 있으니 여유 있게
        float timeout = 10f;
        float t = 0f;
        while (playerStat == null && t < timeout)
        {
            playerStat = GameManager.PlayerManager?.PlayerStat;
            if (playerStat != null) break;
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        if (playerStat == null)
        {
            Debug.LogWarning("[UpgradePersistence] PlayerStat을 찾지 못해 업그레이드 재적용을 건너뜁니다.");
            yield break;
        }

        // 저장된 모든 업그레이드 재적용
        foreach (var id in selectedUpgradeIds)
        {
            ApplyById(id, playerStat);
        }
    }

    /// <summary>
    /// UI에서 업그레이드가 선택되었을 때 호출: 저장 + 즉시 적용
    /// </summary>
    public void SaveAndApply(int id, PlayerStat playerStat)
    {
        if (!selectedUpgradeIds.Contains(id))
            selectedUpgradeIds.Add(id);

        ApplyById(id, playerStat);
    }

    /// <summary>
    /// (씬 재진입 포함) 지정 ID를 PlayerStat에 적용
    /// </summary>
    private void ApplyById(int id, PlayerStat playerStat)
    {
        if (upgradeData == null)
        {
            Debug.LogError("[UpgradePersistence] UpgradeData가 비어있습니다.");
            return;
        }

        if (!upgradeData.Upgrade.ContainsKey(id))
        {
            Debug.LogWarning($"[UpgradePersistence] ID {id}에 해당하는 업그레이드가 UpgradeData에 없습니다.");
            return;
        }

        string effectName = upgradeData.Upgrade[id].Effect; // 예: "FrictionGenerator" 등
        if (string.IsNullOrEmpty(effectName))
        {
            Debug.LogWarning($"[UpgradePersistence] ID {id}의 EffectName이 비어있습니다.");
            return;
        }

        if (effectMap == null || effectMap.Count == 0)
            BuildEffectMap();

        if (effectMap.TryGetValue(effectName, out var effect))
        {
            effect.ApplyEffect(playerStat);
            Debug.Log($"[UpgradePersistence] '{effectName}' 재/적용 완료 (ID: {id})");
        }
        else
        {
            Debug.LogWarning($"[UpgradePersistence] '{effectName}' 효과를 effectsHost에서 찾지 못했습니다.");
        }
    }

    /// <summary>
    /// 필요 시 선택 목록 초기화(예: 챕터 리셋 등)
    /// </summary>
    public void ClearAll()
    {
        selectedUpgradeIds.Clear();
    }

    /// <summary>
    /// 현재 선택 목록 확인용
    /// </summary>
    public IReadOnlyList<int> GetSelectedIds() => selectedUpgradeIds;
}
