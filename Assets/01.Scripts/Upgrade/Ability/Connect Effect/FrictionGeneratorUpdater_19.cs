using _01.Scripts.Manager;
using _01.Scripts.PlayerControll;
using _01.Scripts.PlayerControll.Status;
using UnityEngine;

public class FrictionGeneratorUpdater : MonoBehaviour
{
    private PlayerManager playerManager;
    private PlayerController controller;
    private PlayerStat stat;

    private float chargeRate;
    private float maxCharge;
    private bool isSliding = false;

    public void Initialize(PlayerManager manager, float rate, float max)
    {
        playerManager = manager;
        controller = manager.PlayerController;
        stat = manager.PlayerStat;
        chargeRate = rate;
        maxCharge = max;
    }

    private void Update()
    {
        if (controller == null || stat == null) return;

        // PlayerController 상태 확인
        bool sliding = controller.MovementFSM.CurrentState is SlidingState;

        if (sliding)
        {
            // PlayerStat의 AddOverHeat로 게이지 추가
            stat.AddOverHeat(chargeRate * Time.deltaTime);

            // 최대 30 제한
            if (stat.overheat.Value > maxCharge)
                stat.overheat.Value = maxCharge;

            if (!isSliding)
            {
                isSliding = true;
                Debug.Log("슬라이딩 감지 — 과열 게이지 충전 시작");
            }
        }
        else if (isSliding)
        {
            isSliding = false;
            Debug.Log("슬라이딩 종료 — 과열 충전 중단");
        }
    }
}
