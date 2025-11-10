// IState.cs
// 모든 상태 클래스가 상속받을 공용 인터페이스입니다.
// 각 상태의 진입, 업데이트, 종료 로직을 표준화합니다.
public interface IState
{
    void OnEnter();
    void OnUpdate();
    void OnFixedUpdate(); // Rigidbody 기반 물리 처리를 위해 추가
    void OnExit();
}
