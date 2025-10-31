using _01.Scripts.Manager;

public class GameManager : Singleton<GameManager>
{
  //////////////////////////////////////////////////////////////////////////////
  /// 씬 전환될 때, 파괴되는 스크립트들은 인스턴스를 직접 호출해서 써도 되지만,
  /// 역으로 파괴되지 않는 스크립트들에서 파괴되는 스크립트에 접근하려면 따로 찾아서 써야함.
  /// ex) FindObjectOfType<>()
  //////////////////////////////////////////////////////////////////////////////
  
  #region 게임 핵심 기능 관련 Manager 연결 (GameObject에서 추가되어 파괴되지 않고 계속 쓸 스크립트만)
  SceneManagerEx _sceneManagerEx;
  ResourceManager _resourceManager;
  DataManager _dataManager;
  PlayerManager _playerManager;
  
  public static SceneManagerEx SceneEx { get { return Instance._sceneManagerEx; } }
  public static ResourceManager ResourceEx { get { return Instance._resourceManager; } }
  public static DataManager GameData { get { return Instance._dataManager; } }
  public static PlayerManager PlayerManager { get { return Instance._playerManager; } }
  #endregion  

  protected override void Awake()
  {
    base.Awake();

    if (Instance == this)
    {
      InitManagers();
    }
  }

  // 사용하는 매니저 초기화 하는 함수
  private void InitManagers()
  {
    /////////////////////////////////////////////////////////////////////////////////////////
    /// GameManager Object에 컴포넌트로 들어가 있는 스크립트가 있는 경우 -> GetComponent<>()
    /// 컴포넌트에 들어가 있지 않지만 "파괴되면 안되는 스크립트"를 추가하려는 경우 -> AddComponent<>()
    /////////////////////////////////////////////////////////////////////////////////////////
    
    #region Monobehavior을 상속하지 않은 Manager Class들 초기화 항목
    // ex) _manager = new Manager();
    #endregion

    #region Monobehavior을 상속하는 Manager Class들 초기화 항목
    // ex) _manager gameObject.AddComponent<Manager>();
    _sceneManagerEx = gameObject.GetComponent<SceneManagerEx>();
    _resourceManager = gameObject.GetComponent<ResourceManager>();
    _dataManager = gameObject.GetComponent<DataManager>();
    _playerManager = gameObject.AddComponent<PlayerManager>();
    #endregion
  }
}