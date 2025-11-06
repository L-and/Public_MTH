public static class Constants
{
  ///////////////////////
  /// 문자열 상수화 모음
  /// *** 스크립트 제작 이유 ***
  /// 문자열 값을 직접 입력할때 생기는 오타 때문에
  /// 컴파일 단계에서는 오류를 잡지 못하기 때문에 씁니다.
  /// 
  /// 접근 방법
  /// ex) Constants.ROOM;
  ///////////////////////

  // 맵 Label
  public const string MAP_PROTOTYPE = "Map_Prototype";
  public const string MAP_SEWER = "Map_Sewer";
  public const string MAP_UNDERCASTLE = "Map_UnderCastle";
  public const string MAP_ICECAVE = "Map_IceCave";
  public const string MAP_BOSS = "Map_Boss";

  // Room 구분
  public const string NORMAL_ROOM = "Normal Room";
  public const string BOSS_ROOM = "Boss Room";

  // 파일명 구분
  public const string ROOM = "Room_";
  public const string CONNECTOR = "Connector_";

  // 태그 구분
  public const string PLAYER = "Player";
  public const string ENEMY = "Enemy";
  public const string ELEVATOR = "Elevator";

  // Scene 이름 구분
  public const string CUTSCENE = "CutScene Test";
  public const string MAINMENU = "MainMenu";
  public const string GAMESCENE = "Game Scene";

  // 엘리베이터 구분
  public const string ELEVATOR_PROTOTYPE = "Elevator_Prototype";
  public const string ELEVATOR_SEWER = "Elevator_Sewer";
}
