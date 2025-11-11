using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BossRoomController : MonoBehaviour
{
  [Header("방에 있는 문")]
  [SerializeField] private GameObject _entryDoor;

  [Header("양 문일 경우 (오른쪽)")]
  [SerializeField] private GameObject _entryDoor2;

  [Header("보스 위치 GameObject")]
  [SerializeField] private GameObject _bossPosition;

  private GameObject _bossPrefab; // 보스 프리팹
  private bool isFightStarted = false;  // 전투 시작 여부 체크

  void Start()
  {
    var prefab = GameManager.ResourceEx.bossPrefab;

    if (prefab != null)
      _bossPrefab = prefab;
    else
      Debug.LogWarning("현재 보스 프리팹이 ResourceManagerEx에서 리소스 불러오지 못 했거나, Prefab이 비어있습니다.");
  }

  void OnTriggerEnter(Collider other)
  {
    // 이미 클리어한 방인지, Trigger에 잡힌 Object가 Player 태그를 가지고 있는지 체크
    if (other.CompareTag("Player") && !isFightStarted)
    {
      // 문이 있으면 닫음.
      CloseAllDoor();

      // 한번 들어왔으니 더이상 방 안 Trigger는 필요 없으므로 비활성화
      GetComponent<BoxCollider>().enabled = false;

      // 보스전 시작하는 함수 호출
      StartBossFight();
    }
  }

  // 모든 문이 닫히는 함수
  private void CloseAllDoor()
  {
    if (_entryDoor != null)
    {
      _entryDoor.GetComponent<Door>().Close();

      if (_entryDoor2 != null) _entryDoor2.GetComponent<Door>().Close();
    }
  }

  private void StartBossFight()
  {
    isFightStarted = true;

    // TODO: 보스전 UI 활성화

    // 보스 생성
    Quaternion baseRotation = _bossPosition.transform.rotation;
    Quaternion rotationAmount = Quaternion.Euler(0f, 180f, 0f);
    Instantiate(_bossPrefab, _bossPosition.transform.position, baseRotation * rotationAmount);
  }
}
