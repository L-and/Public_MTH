using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// 엘리베이터 프리팹 찾기 관련 보조 구조체
[Serializable]
public struct StringMapping
{
  public string key;
  public string value;
}

public class ResourceManager : MonoBehaviour
{
  /////////////////////////////////////////////////////
  /// ResourceManager는 리소스 데이터도 관리함.
  /// Handle은 각 리소스마다 각자 사용 할 수 있도록 함.
  /// 필요시 Handle과 리소스 받아서 저장할 변수는 직접 생성.
  /////////////////////////////////////////////////////

  // 핸들 저장할 변수 (다수)
  private AsyncOperationHandle<IList<GameObject>> _mapPrefabLoadHandle;   // 맵
  private AsyncOperationHandle<IList<GameObject>> _enemyPrefabLoadHandle; // 몬스터
  private AsyncOperationHandle<IList<AudioClip>> _SoundLoadHandle;     // 사운드
  private AsyncOperationHandle<IList<GameObject>> _elevatorPrefabLoadHandle;     // 엘리베이터
  // 핸들 저장할 변수 (단일)
  private AsyncOperationHandle<GameObject> _playerPrefabLoadHandle;       // 플레이어


  // 리소스 데이터를 가지고 있는 변수 (다수)
  public Dictionary<string, GameObject> mapPrefabDict { get; private set; }   // 맵
  public Dictionary<string, GameObject> enemyPrefabDict { get; private set; } // 몬스터
  public Dictionary<string, AudioClip> SoundDict { get; private set; } // 사운드
  public GameObject elevatorPrefab { get; private set; }  // 엘리베이터
  private Dictionary<string, GameObject> _elevatorPrefabDict; // 엘리베이터 Dictionary
  // 리소스 데이터를 가지고 있는 변수 (단일)
  public GameObject playerPrefab { get; private set; }    // 플레이어

  // 엘리베이터 관리
  [Header("맵과 엘리베이터 String으로 연결하는 Mapping List")]
  [SerializeField] private List<StringMapping> mapNameMappingList;

  private Dictionary<string, string> mappingDict;

  // 게임 시작하자마자 불러옴.
  async void Awake()
  {
    await LoadPlayerPrefabs();
    await LoadEnemyPrefabs();
    await LoadElevatorPrefabs();
    //await LoadSounds();
        
    // 엘리베이터 데이터 정리
    mappingDict = new Dictionary<string, string>();

    foreach (var mapping in mapNameMappingList)
    {
      if (!mappingDict.ContainsKey(mapping.key))
        mappingDict.Add(mapping.key, mapping.value);
    }
  }

  #region async / await로 리소스 데이터 가져오는 방법
  // public async void LoadPrefabAsync()
  // {
  //   // 리소스 검색할 핸들 값 가져오기.
  //   loadHandle = Addressables.LoadAssetAsync<GameObject>("MyPrefabAddress");

  //   // 로드가 완료될 때까지 비동기 대기
  //   await loadHandle.Task;

  //   if (loadHandle.Status == AsyncOperationStatus.Succeeded)
  //   {
  //     // 프리팹 로드가 성공하면 GameObject 변수에 받아서 사용.
  //     GameObject prefab = loadHandle.Result;
  //     // 불러온 prefab 생성 예시
  //     // Instantiate(prefab);
  //   }
  //   else
  //   {
  //     Debug.LogError("프리팹 로드 실패");
  //   }
  // }
  #endregion

  #region Handle 해제하는 방법
  // 로드한 에셋은 반드시 수동으로 메모리에서 해제 해야 함.
  // 안 그러면 메모리 누수 발생.
  // 해당 함수는 스크립트가 파괴 될 때 로드했던 에셋을 해제
  // void OnDestroy()
  // {
  //   if (mapPrefabLoadHandle.IsValid())
  //   {
  //     Addressables.Release(mapPrefabLoadHandle);
  //   }
  // }
  #endregion

  #region 맵 리소스
  public async Task LoadMapPrefabs(string mapLabel)
  {
    if (mapLabel == null)
    {
      Debug.LogError("Ladel Name이 Null이라 리소스를 받아 올 수 없습니다.");
      return;
    }
    // 0) 있을 수 있는 핸들 해제
    ReleaseMapPrefabs();
    // 1) 리소스 데이터 받을 변수 초기화
    mapPrefabDict = new Dictionary<string, GameObject>();

    // 2) 리소스 데이터 검색할 핸들 가져오기 (key(Label 값), 콜백 null)
    _mapPrefabLoadHandle = Addressables.LoadAssetsAsync<GameObject>(mapLabel, null);

    // 3) 비동기로 불러오기
    await _mapPrefabLoadHandle.Task;

    // 4) 잘 가져왔는지 체크
    if (_mapPrefabLoadHandle.Status == AsyncOperationStatus.Succeeded)
    {
      // 5-1) 잘 가져왔으면 리소스 데이터 변수에 하나씩 추가 (여러개 일 경우)
      foreach (GameObject prefab in _mapPrefabLoadHandle.Result)
        mapPrefabDict.Add(prefab.name, prefab);
    }
    else
    {
      // 5-2) 가져오기 실패 했을 때
      Debug.LogError("맵 프리팹 로딩에 실패했습니다.");
    }
  }

  // 리소스 해제 함수
  public void ReleaseMapPrefabs()
  {
    // 1) 핸들 값을 가지고 있는지 체크
    if (_mapPrefabLoadHandle.IsValid())
    {
      // 2) 핸들 해제
      Addressables.Release(_mapPrefabLoadHandle);
      // 3) 해당 핸들과 연동된 데이터 변수 초기화
      mapPrefabDict.Clear();
    }
  }
  #endregion

  #region 몬스터 리소스 
  public async Task LoadEnemyPrefabs()
  {
    // 0) 있을 수 있는 핸들 해제
    ReleaseEnemyPrefabs();
    // 1) 리소스 데이터 받을 변수 초기화
    enemyPrefabDict = new Dictionary<string, GameObject>();

    // 2) 리소스 데이터 검색할 핸들 가져오기 (key(Label 값), 콜백 null)
    _enemyPrefabLoadHandle = Addressables.LoadAssetsAsync<GameObject>(Constants.ENEMY, null);

    // 3) 비동기로 불러오기
    await _enemyPrefabLoadHandle.Task;

    // 4) 잘 가져왔는지 체크
    if (_enemyPrefabLoadHandle.Status == AsyncOperationStatus.Succeeded)
    {
      // 5-1) 잘 가져왔으면 리소스 데이터 변수에 하나씩 추가 (여러개 일 경우)
      foreach (GameObject prefab in _enemyPrefabLoadHandle.Result)
        enemyPrefabDict.Add(prefab.name, prefab);
    }
    else
    {
      // 5-2) 가져오기 실패 했을 때
      Debug.Log("몬스터 프리팹 로딩에 실패했습니다.");
    }
  }

  // 리소스 해제 함수
  public void ReleaseEnemyPrefabs()
  {
    // 1) 핸들 값을 가지고 있는지 체크
    if (_enemyPrefabLoadHandle.IsValid())
    {
      // 2) 핸들 해제
      Addressables.Release(_enemyPrefabLoadHandle);
      // 3) 해당 핸들과 연동된 데이터 변수 초기화
      enemyPrefabDict.Clear();
    }
  }
  #endregion

  #region 플레이어 리소스
  public async Task LoadPlayerPrefabs()
  {
    // 0) 있을 수 있는 핸들 해제
    ReleasePlayerPrefab();
    // 1) 리소스 데이터 변수 초기화
    playerPrefab = null;

    // 2) 리소스 핸들값을 가져오기 (단일)
    _playerPrefabLoadHandle = Addressables.LoadAssetAsync<GameObject>(Constants.PLAYER);

    // 3) 비동기 실행
    await _playerPrefabLoadHandle.Task;

    // 4) 잘 가져왔는지 체크
    if (_playerPrefabLoadHandle.Status == AsyncOperationStatus.Succeeded)
      playerPrefab = _playerPrefabLoadHandle.Result;
    else
      Debug.LogError("플레이어 프리팹 로드 실패");
  }

  // 리소스 해제
  public void ReleasePlayerPrefab()
  {
    // 핸들이 있는지 확인
    if (_playerPrefabLoadHandle.IsValid())
    {
      Addressables.Release(_playerPrefabLoadHandle);
      playerPrefab = null;
    }
  }
  #endregion

  #region 엘리베이터 리소스
  public async Task LoadElevatorPrefabs()
  {
    // 0) 있을 수 있는 핸들 해제
    ReleaseElevatorPrefab();
    // 1) 리소스 데이터 변수 초기화
    _elevatorPrefabDict = new Dictionary<string, GameObject>();

    // 2) 리소스 핸들값을 가져오기 (단일)
    _elevatorPrefabLoadHandle = Addressables.LoadAssetsAsync<GameObject>(Constants.ELEVATOR, null);

    // 3) 비동기 실행
    await _elevatorPrefabLoadHandle.Task;

    // 4) 잘 가져왔는지 체크
    if (_elevatorPrefabLoadHandle.Status == AsyncOperationStatus.Succeeded)
    {
      foreach (GameObject prefab in _elevatorPrefabLoadHandle.Result)
        _elevatorPrefabDict.Add(prefab.name, prefab);
    }
    else
      Debug.LogError("엘리베이터 프리팹 로드 실패");
  }

  // 리소스 해제
  public void ReleaseElevatorPrefab()
  {
    // 핸들이 있는지 확인
    if (_elevatorPrefabLoadHandle.IsValid())
    {
      Addressables.Release(_elevatorPrefabLoadHandle);
      elevatorPrefab = null;
    }
  }
  #endregion

  #region 사운드 리소스 
  public async Task LoadSounds()
  {
    // 0) 있을 수 있는 핸들 해제
    ReleaseSounds();
    // 1) 리소스 데이터 받을 변수 초기화
    SoundDict = new Dictionary<string, AudioClip>();

    // 2) 리소스 데이터 검색할 핸들 가져오기 (key(Label 값), 콜백 null)
    _SoundLoadHandle = Addressables.LoadAssetsAsync<AudioClip>(Constants.SOUND, null);

    // 3) 비동기로 불러오기
    await _SoundLoadHandle.Task;

    // 4) 잘 가져왔는지 체크
    if (_SoundLoadHandle.Status == AsyncOperationStatus.Succeeded)
    {
      // 5-1) 잘 가져왔으면 리소스 데이터 변수에 하나씩 추가 (여러개 일 경우)
      foreach (AudioClip sound in _SoundLoadHandle.Result)
        SoundDict.Add(sound.name, sound);
    }
    else
    {
      // 5-2) 가져오기 실패 했을 때
      Debug.Log("사운드 로딩에 실패했습니다.");
    }
  }

  // 리소스 해제 함수
  public void ReleaseSounds()
  {
    // 1) 핸들 값을 가지고 있는지 체크
    if (_SoundLoadHandle.IsValid())
    {
      // 2) 핸들 해제
      Addressables.Release(_SoundLoadHandle);
      // 3) 해당 핸들과 연동된 데이터 변수 초기화
      SoundDict.Clear();
    }
  }
  #endregion


  // 엘리베이터 프리팹 내보내는 함수
  public GameObject GetElevatorPrefab(string mapConceptName)
  {
    // 현재 맵 컨셉되 대응되는 엘리베이터 프리팹 이름 가져오기
    string elevatorPrefabName = null;

    if (mappingDict.TryGetValue(mapConceptName, out string elevatorName))
      elevatorPrefabName = elevatorName;

    // 만약 엘리베이터 프리팹 Dictionary가 비어있거나 이름을 찾지 못했을 경우 return
    if (_elevatorPrefabDict == null || elevatorPrefabName == null)
      return null;

    foreach (var fair in _elevatorPrefabDict)
    {
      if (fair.Key == elevatorPrefabName)
        return fair.Value;
    }

    return null;
  }

  void OnDestroy()
  {
    ReleaseMapPrefabs();
    ReleaseEnemyPrefabs();
    ReleasePlayerPrefab();
    ReleaseSounds();
    ReleaseElevatorPrefab();
  }
}