using UnityEngine;

public class GameSceneCanvasControlManager : MonoBehaviour
{
  [Header("GameUI Canvas Object 연결")]
  [SerializeField] private GameObject gameUI;

  [Header("UpgradeUI Canvas Object 연결")]
  [SerializeField] private GameObject upgradeUI;

  void Awake()
  {
    // 시작하면 GameUI만 켜질 수 있도록 호출
    OnGameUI();
  }

  // Canvas가 활성화 되면 이벤트 등록
  void OnEnable()
  {
    elevtest3.OnUpgradeUIEnable += OnUpgradeUI;
  }

  // Canvas가 비활성화 되면 이벤트 등록 해제
  // (현재 게임에서는 Scene이 파괴되면 해제)
  void OnDisable()
  {
    elevtest3.OnUpgradeUIEnable -= OnUpgradeUI;
  }

  // GameUI만 활성화
  private void OnGameUI()
  {
    Debug.Log("OnGameUI");
    if (gameUI != null) gameUI.SetActive(true);
    if (upgradeUI != null) upgradeUI.SetActive(false);
  }

  // UpgradeUI만 활성화
  private void OnUpgradeUI()
  {
    Debug.Log("OnUpgradeUI");
    if (gameUI != null) gameUI.SetActive(false);
    if (upgradeUI != null) upgradeUI.SetActive(true); 
  }
}
