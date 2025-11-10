using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BossRoomController : MonoBehaviour
{
  [Header("방에 있는 문")]
  [SerializeField] private GameObject _entryDoor;

  [Header("양 문일 경우 (오른쪽)")]
  [SerializeField] private GameObject _entryDoor2;
  

  private int totalEnemysToSpawn = 0; // 이 방에 생성될 수 있는 총 몬스터 수
  private int killedEnemysCount = 0;  // 현재 죽은 몬스터 수
  private bool isCleared = false;     // 현재 방이 클리어 되었는지 체크

  void OnTriggerEnter(Collider other)
  {
    // 이미 클리어한 방인지, Trigger에 잡힌 Object가 Player 태그를 가지고 있는지 체크
    if (!isCleared && other.CompareTag("Player"))
    {
      // 문이 있으면 닫음.
      CloseAllDoor();

      // 한번 들어왔으니 더이상 방 안 Trigger는 필요 없으므로 비활성화
      GetComponent<BoxCollider>().enabled = false;
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
}
