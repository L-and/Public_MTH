using _01.Scripts.PlayerControll.Status;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private UpgradeUIManager upgradeUiManager;
    private CanvasGroup upgradeUiCanvasGroup;
    public GameObject upgradeUi;
    private Button button;
    private Vector2 originPosition;
    public UpgradeData upgradeData;

    [SerializeField] private int order;
    private bool isTouchable = false;

    private Tween scaleTween;
    public int dataId;

    void Awake()
    {
        originPosition = transform.localPosition;
        upgradeUiCanvasGroup = upgradeUi.GetComponent<CanvasGroup>();
        upgradeUiManager = upgradeUi.GetComponent<UpgradeUIManager>();
        button = GetComponent<Button>();
    }
    void OnEnable()
    {
        transform.localPosition = originPosition;

        UpgradeOn();
        GetComponent<Image>().DOFade(1, 0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isTouchable)
        {
            transform.DOScale(1, 0);
            scaleTween = transform.DOScale(1.2f, 0.2f);
            upgradeUiManager.SetUpgradeText(dataId); // UpgradeUI 활성화
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        scaleTween.Kill(transform);
        transform.DOScale(1, 0.1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isTouchable)
        {
            Debug.Log("[UpgradeButton] 버튼 활성화"); 
            SetIstouchable(false);
            GameManager.Sound.PlaySFX("Upgrade_Select");
            GlobalMethod.Fade(upgradeUi, 0.4f);
            transform.DOLocalMoveX(0, 0.5f).OnComplete(() =>
            {
                upgradeUiCanvasGroup.DOFade(1, 1.2f).OnComplete(() => upgradeUiCanvasGroup.DOFade(0, 1)).OnComplete(()
                => OnObjectDisable());
            });

            for (int i = 0; i < 3; i++)
            {
                if (order == i) continue;
                Transform child = transform.parent.GetChild(i);
                child.GetComponent<Image>().DOFade(0, 0.2f).OnComplete(() => child.gameObject.SetActive(false));
            }

            // 연주 - ElevatorUpgradeManager로 호출됨
            var manager = Object.FindFirstObjectByType<ElevatorUpgradeManager>();
            if (manager != null)
            {
                manager.ApplyUpgrade(dataId);
                Debug.Log($"[DEBUG] 업그레이드 버튼 활성 완료: ID {dataId}");
            }
            else
            {
                Debug.Log("ElevatorUpgradeManager를 찾을 수 없습니다!");
            }
        }
    }

    void UpgradeOn()
    {
        transform.DOLocalMoveY(0, 1).OnComplete(() => SetIstouchable(true));
    }
    void SetIstouchable(bool value)
    {
        isTouchable = value;
    }

    void OnObjectDisable()
    {
        // 각 층 업그레이드를 1회만 가능하도록 변경
        if (CheckEVHacker() && upgradeUiManager.upgradeChance > 0)
        {   
            upgradeUiManager.upgradeChance--;

            // 엘베 매니저가 다음 층 진입 -> 재활성화 이전까지 다시 열지 않음
            if (upgradeUiManager.upgradeChance <= 0)
            {
                upgradeUi.SetActive(false);
            }
            //upgradeUiManager.UpgradesReActive();
        }
        else upgradeUi.SetActive(false);
    }

    bool CheckEVHacker()
    {
        //Todo 업그레이드 슬롯에 엘리베이터 해커 확인
        return true;
    }

}
