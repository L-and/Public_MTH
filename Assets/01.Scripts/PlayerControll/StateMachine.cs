// StateMachine.cs
// 상태를 관리하고 전환하는 역할을 하는 제네릭 상태 머신 클래스입니다.

using UnityEngine;

public class StateMachine
{
    public IState CurrentState { get; private set; }

    public void Initialize(IState startingState)
    {
        CurrentState = startingState;
        startingState.OnEnter();
    }

    public void ChangeState(IState newState)
    {
        Debug.Log($"이전상태: {CurrentState}, 다음상태: {newState}");
        CurrentState?.OnExit();
        CurrentState = newState;
        CurrentState.OnEnter();
    }
}
