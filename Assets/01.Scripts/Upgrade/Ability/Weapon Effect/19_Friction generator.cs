using _01.Scripts.Manager;
using _01.Scripts.PlayerControll;
using _01.Scripts.PlayerControll.Status;
using UnityEngine;

public class Frictiongenerator_19 : MonoBehaviour, IUpgradeEffect
{
    public string EffectName => "마찰 발전기";

    private PlayerManager playerManager;
    private PlayerStat playerStat;
    private PlayerController playerController;

    private float chargeRate = 2f;   // 초당 게이지 증가량
    private float maxCharge = 30f;   // 최대 충전량
    private bool wasSliding = false; // 슬라이딩 감지용

    public void ApplyEffect(PlayerStat stat)
    {
        playerManager = GameManager.PlayerManager;
        playerStat = playerManager.PlayerStat;
        playerController = playerManager.PlayerController;

        if (playerStat == null || playerController == null)
        {
            Debug.LogWarning(" PlayerStat 또는 PlayerController를 찾을 수 없습니다. 마찰 발전기 적용 불가.");
            return;
        }

        // [김민서] 마찰 발전기 슬라이딩 업그레이드 적용, TODO 동작하지 않으므로 수정필요
        // playerController.SlidingUpgrade();
        
        // [김민서] 슬라이딩시 과열충전 기능이 제대로 동작하지않아, 주석처리함
        // 중복 방지용 보조 스크립트 부착
        // if (playerController.GetComponent<FrictionGeneratorUpdater>() == null)
        // {
        //     var updater = playerController.gameObject.AddComponent<FrictionGeneratorUpdater>();
        //     updater.Initialize(playerManager, chargeRate, maxCharge);
        //     Debug.Log("마찰 발전기 효과 적용됨! 슬라이딩 중 과열 게이지가 초당 2씩, 최대 30까지 충전됩니다.");
        // }
    }
}
