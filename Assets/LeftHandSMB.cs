using UnityEngine;

/// <summary>
/// 플레이어 캐릭터 애니메이션 컨트롤러의 "Leyer Emission"에서 상태전환 시에
/// 보조무기, 방출의 FSM 상태전환 및 오브젝트 조작시점을 알려주는 클래스
/// </summary>
public class LeftHandSMB : StateMachineBehaviour
{
    /// <summary>
    /// Animation State에서 어떤 동작이 실행되어야 하는지를 명시하는 Enum
    /// </summary>
    enum EActions
    {
        SetLeftHandIdle, // 왼손을 사용하는 애니메이션이 끝남 (Default State)
        EmissionFire, // 방출의 기능적인 동작이 실행됨 (그랩 던지기, 레일일건 발사)
        EmissionLower // 방출의 애니메이션이 끝남
    }

    [SerializeField] EActions _actionType;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        if (_actionType == EActions.SetLeftHandIdle)
            GameManager.PlayerManager?.PlayerController?.SetLeftHandStateCooldown(); // Default State로 진입하면 왼손(보조무기, 방출)을 Cooldown 상태로 전환
        else if (_actionType == EActions.EmissionFire)
            GameManager.PlayerManager?.PlayerController?.FireEmission();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 방출사용이 종료되면 방출무기를 꺼줌
        if (_actionType == EActions.EmissionLower)
            GameManager.PlayerManager?.PlayerController?.playerEmission.ModelSetActive(false);
    }
    
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
    
    

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
